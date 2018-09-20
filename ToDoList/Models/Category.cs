using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ToDoList;
using System;

namespace ToDoList.Models
{
  public class Category
  {
    public string Name { get; set; }
    public int Id { get; }

    public Category(string categoryName, int id = 0)
    {
      Name = categoryName;
      Id = id;
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
        bool descriptionEquality = (this.Name == newCategory.Name);
        return (descriptionEquality);
      }
    }

    public override int GetHashCode()
    {
         return this.Name.GetHashCode();
    }

    public static List<Category> GetAll()
    {
      List<Category> allCategories = new List<Category> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM categories;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        string categoryName = rdr.GetString(1);
        int categoryId = rdr.GetInt32(0);

        Category newCategory = new Category(categoryName, categoryId);
        allCategories.Add(newCategory);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCategories;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM categories;";

      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {

    }

    public static Category Find(int searchId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM categories WHERE id = @searchId;";

      MySqlParameter parameterId = new MySqlParameter();
      parameterId.ParameterName = "@searchId";
      parameterId.Value = searchId;
      cmd.Parameters.Add(parameterId);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

      string categoryName = "";
      int categoryId = 0;

      if(rdr.Read())
      {
        categoryName = rdr.GetString(1);
        categoryId = rdr.GetInt32(0);
      }

      Category newCategory = new Category(categoryName, categoryId);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

      return newCategory;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO categories(name) VALUES (@name);";
      MySqlParameter parameterName = new MySqlParameter();
      parameterName.ParameterName = "@name";
      parameterName.Value = this.Name;
      cmd.Parameters.Add(parameterName);

      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Item> GetItems()
    {
      List<Item> newItems = new List<Item> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items WHERE category_id=@id ORDER BY duedate;";
      MySqlParameter parameterId = new MySqlParameter();
      parameterId.ParameterName = "@id";
      parameterId.Value = this.Id;
      cmd.Parameters.Add(parameterId);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        string itemDescription = rdr.GetString(1);

        Item newItem;
        if(rdr.IsDBNull(3))
        {
          //if current row's duedate column's value is null
          newItem = new Item(itemDescription, itemId, this.Id);
        }
        else
        {
          //if current row's duedate column's value is not null and has specific value
          DateTime itemDueDate = rdr.GetDateTime(3);
          newItem = new Item(itemDescription, itemId, this.Id, itemDueDate);
        }

        newItems.Add(newItem);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newItems;
    }
  }
}
