using Library01.Data;
using Library01.Models;
using System.Linq;
using System.Windows;

namespace Library01.Views
{
    public partial class GenresWindow : Window
    {
        public GenresWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using var db = new LibraryContext();
            DgGenres.ItemsSource = db.Genres.OrderBy(g => g.Name).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new GenreEditWindow(null);
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgGenres.SelectedItem is not Genre genre)
            {
                MessageBox.Show("Выберите жанр.", "Внимание");
                return;
            }
            var win = new GenreEditWindow(genre.Id);
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgGenres.SelectedItem is not Genre genre)
            {
                MessageBox.Show("Выберите жанр.", "Внимание");
                return;
            }
            if (MessageBox.Show($"Удалить жанр «{genre.Name}»?",
                "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using var db = new LibraryContext();
            var g = db.Genres.Find(genre.Id);
            if (g != null)
            {
                db.Genres.Remove(g);
                db.SaveChanges();
            }
            LoadData();
        }
    }
}
