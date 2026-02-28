using Library01.Data;
using Library01.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace Library01.Views
{
    public partial class AuthorEditWindow : Window
    {
        private readonly int? _authorId;

        public AuthorEditWindow(int? authorId)
        {
            InitializeComponent();
            _authorId = authorId;
            Title = authorId == null ? "Добавить автора" : "Редактировать автора";

            if (authorId != null)
            {
                using var db = new LibraryContext();
                var a = db.Authors.Find(authorId);
                if (a != null)
                {
                    TxtLastName.Text = a.LastName;
                    TxtFirstName.Text = a.FirstName;
                    TxtBirthYear.Text = a.BirthYear?.ToString();
                    TxtCountry.Text = a.Country;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var lastName = TxtLastName.Text.Trim();
            var firstName = TxtFirstName.Text.Trim();

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Введите фамилию и имя.", "Ошибка");
                return;
            }

            int? birthYear = null;
            if (!string.IsNullOrWhiteSpace(TxtBirthYear.Text))
            {
                if (!int.TryParse(TxtBirthYear.Text, out int y))
                {
                    MessageBox.Show("Год рождения должен быть числом.", "Ошибка");
                    return;
                }
                birthYear = y;
            }

            using var db = new LibraryContext();

            // Проверка уникальности
            var duplicate = db.Authors.Any(a =>
                a.LastName == lastName &&
                a.FirstName == firstName &&
                a.BirthYear == birthYear &&
                a.Id != (_authorId ?? 0));

            if (duplicate)
            {
                MessageBox.Show("Автор с такими данными уже существует.", "Ошибка");
                return;
            }

            Author author;
            if (_authorId == null)
            {
                author = new Author();
                db.Authors.Add(author);
            }
            else
            {
                author = db.Authors.Find(_authorId)!;
            }

            author.LastName = lastName;
            author.FirstName = firstName;
            author.BirthYear = birthYear;
            author.Country = string.IsNullOrWhiteSpace(TxtCountry.Text) ? null : TxtCountry.Text.Trim();

            db.SaveChanges();
            DialogResult = true;
        }
    }
}
