using System;
using System.Windows;
using System.Collections.Generic;
using PetaPoco;

namespace PetaPocoSample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoadData();
        }

        /// <summary>
        /// Load data from database
        /// </summary>
        private void LoadData()
        {
            using (var db = new Database("SqliteConnStr"))
            {
                //1. Insert data
                var listData = new List<article>
                {
                    new article { article_id=111, title="hello 111", date_created = DateTime.Now, draft= true, content="sample 1111." },
                    new article { article_id=222, title="hello 222", date_created = DateTime.Now, draft= false, content="sample 2222."},
                    new article { article_id=333, title="hello 333", date_created = DateTime.Now, draft= true, content="sample 3333."},
                    new article { article_id=444, title="hello 444", date_created = DateTime.Now, draft= false, content="sample 4444."},
                    new article { article_id=555, title="hello 555", date_created = DateTime.Now, draft= true, content="sample 5555."},
                };

                listData.ForEach(p => db.Insert(p));

                //2. Fetch data
                dataGrid1.ItemsSource = db.Fetch<article>("select * from article");

            }
        }

        [TableName("article")]
        [PrimaryKey("article_id")]
        public class article
        {
            public long article_id { get; set; }
            public string title { get; set; }
            public DateTime date_created { get; set; }
            public bool draft { get; set; }
            public string content { get; set; }
        }
    }
}
