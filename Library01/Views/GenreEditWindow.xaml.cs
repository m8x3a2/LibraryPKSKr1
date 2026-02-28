using Library01.Data;
using Library01.Models;
using System.Linq;
using System.Windows;

namespace Library01.Views
{
    public partial class GenreEditWindow : Window
    {
        private readonly int? _genreId;

        public GenreEditWindow(int? genreId)
        {
            InitializeComponent();
            _genreId = genreId;
            Title = genreId == null ? "Добавить жанр" : "Редактировать жанр";

            if (genreId != null)
            {
                using var db = new LibraryContext();
                var g = db.Genres.Find(genreId);
                if (g != null)
                {
                    TxtName.Text = g.Name;
                    TxtDescription.Text = g.Description;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var name = TxtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название жанра.", "Ошибка");
                return;
            }

            using var db = new LibraryContext();

            // Проверка уникальности
            var duplicate = db.Genres.Any(g =>
                g.Name == name && g.Id != (_genreId ?? 0));

            if (duplicate)
            {
                MessageBox.Show("Жанр с таким названием уже существует.", "Ошибка");
                return;
            }

            Genre genre;
            if (_genreId == null)
            {
                genre = new Genre();
                db.Genres.Add(genre);
            }
            else
            {
                genre = db.Genres.Find(_genreId)!;
            }

            genre.Name = name;
            genre.Description = string.IsNullOrWhiteSpace(TxtDescription.Text)
                ? null : TxtDescription.Text.Trim();

            db.SaveChanges();
            DialogResult = true;
        }
    }
}
