using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace ToDoList.Models
{
    public class Category
    {
        private string _name;
        private int _id;
        public Category(string name, int id = 0)
        {
            _name = name;
            _id = id;
        }
        public override bool Equals(System.Object otherCategory)
        {
            if (!(otherCategory is Category))
            {
                return false;
            }
            else
            {
                Category newCategory = (Category) otherCategory;
                bool idEquality = this.GetId().Equals(newCategory.GetId());
                bool nameEquality = this.GetName().Equals(newCategory.GetName());
                return (idEquality && nameEquality);
            }
        }
        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }
        public string GetName()
        {
            return _name;
        }
        public int GetId()
        {
            return _id;
        }
        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO category (name) VALUES (@name);";

            MySqlParameter name = new MySqlParameter();
            name.ParameterName = "@name";
            name.Value = this._name;
            cmd.Parameters.Add(name);

            cmd.ExecuteNonQuery();
            _id = (int) cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

        }
        public static List<Category> GetAll()
        {
            List<Category> allCategories = new List<Category> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM category;";
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
              int CategoryId = rdr.GetInt32(0);
              string categoryDescription = rdr.GetString(1);
              Category newCategory = new Category(categoryDescription, CategoryId);
              allCategories.Add(newCategory);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allCategories;
        }
        public static Category Find(int id)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM category WHERE id = (@searchId);";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = id;
            cmd.Parameters.Add(searchId);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            int CategoryId = 0;
            string categoryDescription = "";

            while(rdr.Read())
            {
              CategoryId = rdr.GetInt32(0);
              categoryDescription = rdr.GetString(1);
            }
            Category newCategory = new Category(categoryDescription, CategoryId);
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return newCategory;
        }

        public List<Item> GetItems()
        {
          List<Item> allCategoryItems = new List<Item> {};
          MySqlConnection conn = DB.Connection();
          conn.Open();
          var cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"SELECT * FROM item WHERE categoryId = @category.Id;";
          MySqlParameter categoryId = new MySqlParameter();
          categoryId.ParameterName = "@category.Id";
          categoryId.Value = this._id;
          cmd.Parameters.Add(categoryId);
          var rdr = cmd.ExecuteReader() as MySqlDataReader;
          while(rdr.Read())
          {
            int itemId = rdr.GetInt32(0);
            string itemDescription = rdr.GetString(1);
            DateTime itemDueDate = rdr.GetDateTime(2);
            int itemCategoryId = rdr.GetInt32(3);
            Item newItem = new Item(itemDescription, itemDueDate, itemId, itemCategoryId);
            allCategoryItems.Add(newItem);
          }
          conn.Close();
          if (conn != null)
          {
            conn.Dispose();
          }
          return allCategoryItems;
        }

        public static void DeleteAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM category;";
            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
    }
}
