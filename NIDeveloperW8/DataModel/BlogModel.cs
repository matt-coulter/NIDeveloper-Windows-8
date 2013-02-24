using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace NIDeveloperW8.Data
{
    public class BlogModel : INotifyPropertyChanged
    {
        public BlogModel()
        {
            this.Items = new ObservableCollection<BlogPostModel>();
            this.Categories = new ObservableCollection<CategoryModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BlogPostModel> Items { get; private set; }
        public ObservableCollection<CategoryModel> Categories { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public async Task<string> getBlogPosts()
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "http://www.nideveloper.co.uk/json/index/18";
                HttpResponseMessage response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> getCategories()
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "http://www.nideveloper.co.uk/json/categories";
                HttpResponseMessage response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static object Deserialize(Stream streamObject, Type serializedObjectType)
        {
            if (serializedObjectType == null || streamObject == null)
                return null;

            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializedObjectType);
            return ser.ReadObject(streamObject);
        }

        private void convertJSONtoCategoryModel(string results)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                var myStr = results;
                ms.Write(StrToByteArray(myStr), 0, myStr.Length);
                ms.Position = 0;

                // deserialization
                BlogCategoryContainer data = (BlogCategoryContainer)Deserialize(ms, typeof(BlogCategoryContainer));
                foreach (CategoryModel category in data.categories)
                {
                    this.Categories.Add(category);
                }
            }
            catch (Exception exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    showMessage(exception.Message, "Error");
                }
                else
                {
                    showMessage("Please check that you have an internet connection", "Error");
                }
            }
            finally
            {
                ms.Dispose();
            }
        }

        private async void showMessage(string message, string title)
        {
            Windows.UI.Popups.MessageDialog dialogue = new Windows.UI.Popups.MessageDialog(message, title);
            await dialogue.ShowAsync();
        }

        private void convertJSONtoModel(string results)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                var myStr = results;
                ms.Write(StrToByteArray(myStr), 0, myStr.Length);
                ms.Position = 0;

                // deserialization
                BlogJsonContainer data = (BlogJsonContainer)Deserialize(ms, typeof(BlogJsonContainer));
                foreach (BlogPostModel post in data.posts)
                {
                    int categoryID = post.Category;
                    foreach (CategoryModel category in Categories)
                    {
                        if (category.ID == categoryID)
                        {
                            post.CategoryName = category.Name;
                        }
                    }
                    this.Items.Add(post);
                }
            }
            catch (Exception exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    showMessage(exception.Message, "Error");
                }
                else
                {
                    showMessage("Please check that you have an internet connection", "Error");
                }
            }
            finally
            {
                ms.Dispose();
            }
        }

        internal async void loadData()
        {
            convertJSONtoCategoryModel(await getCategories());
            convertJSONtoModel(await getBlogPosts());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
