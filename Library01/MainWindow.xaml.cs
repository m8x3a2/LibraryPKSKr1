using Library01.Data;
using Library01.Models;
using Library01.ViewModels;
using Library01.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Library01
{
    public partial class MainWindow : Window
    {
        private List<Book> _allBooks = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var db = new LibraryContext();
            db.Database.EnsureCreated();
            LoadData();
        }

        private void LoadData()
        {
            using var db = new LibraryContext();

            _allBooks = db.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .ToList();

            // Обновляем ComboBox авторов
            var selectedAuthor = CmbAuthor.SelectedItem as Author;
            var authors = db.Authors.OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToList();
            CmbAuthor.ItemsSource = new List<Author?> { null }.Concat(authors).ToList();
            CmbAuthor.DisplayMemberPath = null; // используем ToString()
            CmbAuthor.SelectedIndex = 0;

            // Обновляем ComboBox жанров
            var selectedGenre = CmbGenre.SelectedItem as Genre;
            var genres = db.Genres.OrderBy(g => g.Name).ToList();
            CmbGenre.ItemsSource = new List<Genre?> { null }.Concat(genres).ToList();
            CmbGenre.SelectedIndex = 0;

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var search = TxtSearch.Text.Trim().ToLower();
            var filterAuthor = CmbAuthor.SelectedItem as Author;
            var filterGenre = CmbGenre.SelectedItem as Genre;

            var filtered = _allBooks.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(b => b.Title.ToLower().Contains(search));

            if (filterAuthor != null)
                filtered = filtered.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == filterAuthor.Id));

            if (filterGenre != null)
                filtered = filtered.Where(b => b.BookGenres.Any(bg => bg.GenreId == filterGenre.Id));

            DgBooks.ItemsSource = filtered.Select(BookViewModel.FromBook).ToList();
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
            => ApplyFilter();

        private void Filter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            => ApplyFilter();

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "";
            CmbAuthor.SelectedIndex = 0;
            CmbGenre.SelectedIndex = 0;
        }

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            var win = new BookEditWindow(null);
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            if (DgBooks.SelectedItem is not BookViewModel vm)
            {
                MessageBox.Show("Выберите книгу для редактирования.", "Внимание");
                return;
            }
            var win = new BookEditWindow(vm.Id);
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (DgBooks.SelectedItem is not BookViewModel vm)
            {
                MessageBox.Show("Выберите книгу для удаления.", "Внимание");
                return;
            }
            if (MessageBox.Show($"Удалить книгу «{vm.Title}»?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using var db = new LibraryContext();
            var book = db.Books.Find(vm.Id);
            if (book != null)
            {
                db.Books.Remove(book);
                db.SaveChanges();
            }
            LoadData();
        }

        private void BtnAuthors_Click(object sender, RoutedEventArgs e)
        {
            var win = new AuthorsWindow();
            win.ShowDialog();
            LoadData();
        }

        private void BtnGenres_Click(object sender, RoutedEventArgs e)
        {
            var win = new GenresWindow();
            win.ShowDialog();
            LoadData();
        }
    }
}
