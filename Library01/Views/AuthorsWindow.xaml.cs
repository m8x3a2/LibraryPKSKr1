using Library01.Data;
using Library01.Models;
using System.Linq;
using System.Windows;

namespace Library01.Views
{
    public partial class AuthorsWindow : Window
    {
        public AuthorsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using var db = new LibraryContext();
            DgAuthors.ItemsSource = db.Authors
                .OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
                .ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new AuthorEditWindow(null);
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgAuthors.SelectedItem is not Author author)
            {
                MessageBox.Show("Выберите автора.", "Внимание");
                return;
            }
            var win = new AuthorEditWindow(author.Id);
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgAuthors.SelectedItem is not Author author)
            {
                MessageBox.Show("Выберите автора.", "Внимание");
                return;
            }
            if (MessageBox.Show($"Удалить автора «{author.LastName} {author.FirstName}»?",
                "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using var db = new LibraryContext();
            var a = db.Authors.Find(author.Id);
            if (a != null)
            {
                db.Authors.Remove(a);
                db.SaveChanges();
            }
            LoadData();
        }
    }
}
