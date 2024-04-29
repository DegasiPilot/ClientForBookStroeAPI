using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientForMyAPI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private APIAgent APIAgent = new();

         public MainWindow()
        {
            InitializeComponent();
            BooksDataGrid.ItemsSource = APIAgent.GetBooks();
        }

        private void bookIdTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(bookIdTB.Text)) 
            { 
                BooksDataGrid.ItemsSource = null;
                BooksDataGrid.ItemsSource = APIAgent.GetBooks();
            }
            else
            {
                BooksDataGrid.ItemsSource = null;
                var books = APIAgent.GetBooks().Where(x => x.Title.ToLower().Contains(bookIdTB.Text.ToLower()));
                BooksDataGrid.ItemsSource = books;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if(BooksDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Сначала нажмите на строку которую хотите удалить!");
                return;
            }
            var selectedBook = (BooksDataGrid.SelectedItem as Book);
            APIAgent.DeleteBook(selectedBook.Id);
            bookIdTB_TextChanged(null, null);
            ClearInfoInTextBoxes();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetBookFromTextBoxes(out Book book))
            {
                APIAgent.AddBook(book);
                bookIdTB_TextChanged(null, null);
                ClearInfoInTextBoxes();
            }
        }

        private void BooksDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Any())
            {
                UpdateInfoInTextBoxes(e.AddedCells[0].Item as Book);
            }
        }

        private void UpdateInfoInTextBoxes(Book book)
        {
            IdTb.Text = book.Id.ToString();
            TitleTb.Text = book.Title;
            AuthorTb.Text = book.Author;
            PagesCountTb.Text = book.PagesCount.ToString();
            PriceTb.Text = book.Price.ToString();
        }

        private void ClearInfoInTextBoxes()
        {
            IdTb.Clear();
            TitleTb.Clear();
            AuthorTb.Clear();
            PagesCountTb.Clear();
            PriceTb.Clear();
        }

        private void ChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            if(TryGetBookFromTextBoxes(out Book book))
            {
                if (int.TryParse(IdTb.Text, out int id))
                {
                    book.Id = id;
                    APIAgent.PutBook(book);
                    bookIdTB_TextChanged(null, null);
                    ClearInfoInTextBoxes();
                }
                else
                {
                    MessageBox.Show("Вы не выбрали книгу для редактирования!");
                }
            }
        }

        private bool TryGetBookFromTextBoxes(out Book book)
        {
            if (string.IsNullOrEmpty(TitleTb.Text))
            {
                MessageBox.Show("Введите название книги!");
                book = new Book();
                return false;
            }

            book = new Book()
            {
                Title = TitleTb.Text,
                Author = AuthorTb.Text,
            };

            if (int.TryParse(PagesCountTb.Text, out int pagesCount))
            {
                book.PagesCount = pagesCount;
            }
            else
            {
                MessageBox.Show("Неправильное значение в поле Pages count");
                return false;
            }
            if (decimal.TryParse(PriceTb.Text, out decimal price))
            {
                book.Price = price;
            }
            else
            {
                MessageBox.Show("Неправильное значение в поле Price");
                return false;
            }
            return true;
        }
    }
}
