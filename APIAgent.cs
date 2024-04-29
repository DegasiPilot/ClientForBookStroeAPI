using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace ClientForMyAPI
{
    internal class APIAgent
    {
        HttpClient client = new HttpClient();
        private readonly string url = @"https://localhost:7125/";

        public IEnumerable<Book> GetBooks()
        {
            return ConvertFromJson<IEnumerable<Book>>(MakeGetRequestAsync(url + "books"));
        }

        public IEnumerable<Book> GetBookId(string id)
        {
            return ConvertFromJson<IEnumerable<Book>>(MakeGetRequestAsync(url + $"book/{id}"));
        }

        public void AddBook(Book book)
        {
            MakePostRequestAsync(url + "book", book);
        }

        public void PutBook(Book book)
        {
            var status = MakePutRequestAsync(url + "book", book);
            if(status == HttpStatusCode.NotFound)
            {
                MessageBox.Show("Этой книги не существует!");
            }
        }

        public void DeleteBook(int id)
        {
            MessageBox.Show(MakeDeleteRequestAsync(url + $"book", id).ToString());
        }
        private T ConvertFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        private string MakePostRequestAsync(string url, Book book)
        {
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;
            var httpStatusCode = response.StatusCode;
            if (httpStatusCode != HttpStatusCode.OK)
            {
                return "";
            }
            return response.Content.ReadAsStringAsync().Result;
        }
        private string MakeGetRequestAsync(string url)
        {
            var response = client.GetAsync(url).Result;
            var httpStatusCode = response.StatusCode;
            if (httpStatusCode != HttpStatusCode.OK)
            {
                return "";
            }
            return response.Content.ReadAsStringAsync().Result;
        }

        private HttpStatusCode MakePutRequestAsync(string url, Book book)
        {
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var response = client.PutAsync(url, content).Result;
            return response.StatusCode;
        }
        private HttpStatusCode MakeDeleteRequestAsync(string url, int id)
        {
            var response = client.DeleteAsync(url + $"/{id}").Result;
            return response.StatusCode;
        }
    }
}
