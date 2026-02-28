using Library01.Data;
using Library01.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Library01.Views
{
    public partial class BookEditWindow : Window
    {
        private readonly int? _bookId;

        // Валидация ISBN-13:
        // Формат: xxx-x-xxx-xxxxx-x (13 цифр, разделённых дефисами)
        // Группы: 3-1-3-5-1
        private static bool IsValidIsbn13(string input, out string normalized)
        {
            normalized = string.Empty;
            var s = input.Trim();

            // Принимаем формат с дефисами: 3-1-3-5-1
            var match = Regex.Match(s, @"^(\d{3})-(\d{1})-(\d{3})-(\d{5})-(\d{1})$");
            if (!match.Success)
                return false;

            normalized = s; // формат уже правильный
            return true;
        }

        // Вспомогательный класс для отображения автора в ListBox
        private class AuthorItem
        {
            public int Id { get; set; }
            public string DisplayName { get; set; } = "";
        }

        public BookEditWindow(int? bookId)
        {
            InitializeComponent();
            _bookId = bookId;
            Title = bookId == null ? "Добавить книгу" : "Редактировать книгу";
            LoadLists();
        }

        private void LoadLists()
        {
            using var db = new LibraryContext();

            var authors = db.Authors.OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
                .Select(a => new AuthorItem { Id = a.Id, DisplayName = $"{a.LastName} {a.FirstName}" })
                .ToList();
            LbAuthors.ItemsSource = authors;

            var genres = db.Genres.OrderBy(g => g.Name).ToList();
            LbGenres.ItemsSource = genres;

            if (_bookId != null)
            {
                var book = db.Books
                    .Include(b => b.BookAuthors)
                    .Include(b => b.BookGenres)
                    .FirstOrDefault(b => b.Id == _bookId);

                if (book != null)
                {
                    TxtTitle.Text = book.Title;
                    TxtYear.Text = book.PublishYear?.ToString();
                    TxtISBN.Text = book.ISBN;
                    TxtQty.Text = book.QuantityInStock.ToString();

                    var bookAuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();
                    foreach (AuthorItem item in LbAuthors.Items)
                        if (bookAuthorIds.Contains(item.Id))
                            LbAuthors.SelectedItems.Add(item);

                    var bookGenreIds = book.BookGenres.Select(bg => bg.GenreId).ToHashSet();
                    foreach (Genre g in LbGenres.Items)
                        if (bookGenreIds.Contains(g.Id))
                            LbGenres.SelectedItems.Add(g);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var title = TxtTitle.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введите название книги.", "Ошибка");
                return;
            }

            int? year = null;
            if (!string.IsNullOrWhiteSpace(TxtYear.Text))
            {
                if (!int.TryParse(TxtYear.Text, out int y))
                {
                    MessageBox.Show("Год должен быть числом.", "Ошибка");
                    return;
                }
                year = y;
            }

            int qty = 0;
            if (!string.IsNullOrWhiteSpace(TxtQty.Text))
            {
                if (!int.TryParse(TxtQty.Text, out qty) || qty < 0)
                {
                    MessageBox.Show("Количество должно быть неотрицательным числом.", "Ошибка");
                    return;
                }
            }

            // Валидация ISBN
            string? isbnValue = null;
            if (!string.IsNullOrWhiteSpace(TxtISBN.Text))
            {
                if (!IsValidIsbn13(TxtISBN.Text, out string normalizedIsbn))
                {
                    var result = MessageBox.Show(
                        "Неверный формат ISBN-13.\n\n" +
                        "Правильный формат: 978-5-699-12014-7\n" +
                        "  • 978 или 979 — префикс EAN.UCC\n" +
                        "  • 5 — номер регистрационной группы\n" +
                        "  • 699 — идентификатор издателя\n" +
                        "  • 12014 — номер издания\n" +
                        "  • 7 — контрольная цифра\n\n" +
                        "Исправьте ISBN или нажмите «Да», чтобы сохранить без него.",
                        "Предупреждение — неверный ISBN",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                        return;

                    isbnValue = null; // сохраняем без ISBN
                }
                else
                {
                    isbnValue = normalizedIsbn;
                }
            }

            var selectedAuthorIds = LbAuthors.SelectedItems.Cast<AuthorItem>().Select(a => a.Id).ToList();
            var selectedGenreIds = LbGenres.SelectedItems.Cast<Genre>().Select(g => g.Id).ToList();

            using var db = new LibraryContext();

            Book book;
            if (_bookId == null)
            {
                book = new Book();
                db.Books.Add(book);
            }
            else
            {
                book = db.Books
                    .Include(b => b.BookAuthors)
                    .Include(b => b.BookGenres)
                    .First(b => b.Id == _bookId);

                db.BookAuthors.RemoveRange(book.BookAuthors);
                db.BookGenres.RemoveRange(book.BookGenres);
            }

            // Проверка уникальности ISBN перед сохранением
            if (isbnValue != null)
            {
                var duplicate = db.Books.Any(b => b.ISBN == isbnValue && b.Id != (_bookId ?? 0));
                if (duplicate)
                {
                    MessageBox.Show(
                        $"Книга с ISBN {isbnValue} уже существует в базе.",
                        "Дубликат ISBN",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }

            book.Title = title;
            book.PublishYear = year;
            book.ISBN = isbnValue;
            book.QuantityInStock = qty;

            db.SaveChanges();

            foreach (var aid in selectedAuthorIds)
                db.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = aid });

            foreach (var gid in selectedGenreIds)
                db.BookGenres.Add(new BookGenre { BookId = book.Id, GenreId = gid });

            db.SaveChanges();

            DialogResult = true;
        }
    }
}
