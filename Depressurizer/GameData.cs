﻿/*
Copyright 2011, 2012, 2013 Steve Labbe.

This file is part of Depressurizer.

Depressurizer is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Depressurizer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Depressurizer.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Depressurizer.Lib;
using Depressurizer.Properties;
using Depressurizer.VdfFile;
using ValueType = Depressurizer.VdfFile.ValueType;

namespace Depressurizer
{
    /// <summary>
    ///     Represents a single game and its categories.
    /// </summary>
    public class GameInfo
    {
        #region Fields

        public SortedSet<Category> Categories;
        public bool Hidden;
        public int Id; // Positive ID matches to a Steam ID, negative means it's a non-steam game (= -1 - shortcut ID)
        public string Name;

        private string _launchStr;

        /// <summary>
        ///     ID String to use to launch this game. Uses the ID for steam games, but non-steam game IDs need to be set.
        /// </summary>
        public string LaunchString
        {
            get
            {
                if (Id > 0) return Id.ToString();
                if (!string.IsNullOrEmpty(_launchStr)) return _launchStr;
                return null;
            }
            set { _launchStr = value; }
        }

        #endregion

        /// <summary>
        ///     Construct a new GameInfo with no categories set.
        /// </summary>
        /// <param name="id">ID of the new game. Positive means it's the game's Steam ID, negative means it's a non-steam game.</param>
        /// <param name="name">Game title</param>
        public GameInfo(int id, string name)
        {
            Id = id;
            Name = name;
            Hidden = false;
            Categories = new SortedSet<Category>();
        }

        #region Category Modifiers

        /// <summary>
        ///     Adds a single category to this game. Does nothing if the category is already attached.
        /// </summary>
        /// <param name="newCat">Category to add</param>
        public void AddCategory(Category newCat)
        {
            if (newCat != null) Categories.Add(newCat);
        }

        /// <summary>
        ///     Adds a list of categories to this game. Skips categories that are already attached.
        /// </summary>
        /// <param name="newCats">A list of categories to add</param>
        public void AddCategory(ICollection<Category> newCats)
        {
            Categories.UnionWith(newCats);
        }

        /// <summary>
        ///     Removes a single category from this game. Does nothing if the category is not attached to this game.
        /// </summary>
        /// <param name="remCat">Category to remove</param>
        public void RemoveCategory(Category remCat)
        {
            Categories.Remove(remCat);
        }

        /// <summary>
        ///     Removes a list of categories from this game. Skips categories that are not attached to this game.
        /// </summary>
        /// <param name="remCats">Categories to remove</param>
        public void RemoveCategory(ICollection<Category> remCats)
        {
            Categories.ExceptWith(remCats);
        }

        /// <summary>
        ///     Removes all categories from this game.
        /// </summary>
        public void ClearCategories()
        {
            Categories.Clear();
        }

        /// <summary>
        ///     Remove all categories attached to this game except for the specified list
        /// </summary>
        /// <param name="exceptions">List of categories to leave in place</param>
        public void ClearCategoriesExcept(ICollection<Category> exceptions)
        {
            Categories.IntersectWith(exceptions);
        }

        /// <summary>
        ///     Remove all categories attached to this game except for the specified one
        /// </summary>
        /// <param name="c">Category to leave in place</param>
        public void ClearCategoriesExcept(Category c)
        {
            bool restore = Categories.Contains(c);
            Categories.Clear();
            if (restore)
            {
                Categories.Add(c);
            }
        }

        /// <summary>
        ///     Sets the categories for this game to exactly match the given list. Missing categories will be added and extra ones
        ///     will be removed.
        /// </summary>
        /// <param name="cats">Set of categories to apply to this game</param>
        public void SetCategories(ICollection<Category> cats)
        {
            ClearCategories();
            AddCategory(cats);
        }

        #endregion

        #region Accessors

        /// <summary>
        ///     Check whether the game includes the given category
        /// </summary>
        /// <param name="c">Category to look for</param>
        /// <returns>True if category is found</returns>
        public bool ContainsCategory(Category c)
        {
            return Categories.Contains(c);
        }

        /// <summary>
        ///     Check to see if the game has any categories at all
        /// </summary>
        /// <returns>True if the category set is not empty</returns>
        public bool HasCategories()
        {
            return Categories.Count > 0;
        }

        /// <summary>
        ///     Check to see if the game has any categories at all, besides the given category
        /// </summary>
        /// <param name="c">Category to except from the check</param>
        /// <returns>True if the game has any categories set besides c</returns>
        public bool HasCategoriesExcept(Category c)
        {
            if (Categories.Count == 0) return false;
            if (Categories.Count == 1 && Categories.Contains(c)) return false;
            return true;
        }

        /// <summary>
        ///     Check to see if the game has any categories set that do not exist in the given list
        /// </summary>
        /// <param name="except">List of games to exclude from the  check</param>
        /// <returns>True if the game has any categories that do not exist in the list</returns>
        public bool HasCategoriesExcept(ICollection<Category> except)
        {
            if (Categories.Count == 0) return false;
            return Categories.Any(c => !except.Contains(c));
        }

        /// <summary>
        ///     Gets a string listing the game's assigned categories.
        /// </summary>
        /// <param name="ifEmpty">Value to return if there are no categories</param>
        /// <returns>List of the game's categories, separated by commas.</returns>
        public string GetCatString(string ifEmpty = "")
        {
            string result = "";
            bool first = true;
            foreach (Category c in Categories)
            {
                if (first)
                {
                    result += ", ";
                }
                result += c.Name;
                first = false;
            }
            return first ? ifEmpty : result;
        }

        /// <summary>
        ///     Gets a string listing the game's assigned categories, omitting the given category from the list if it is found.
        /// </summary>
        /// <param name="except">Category to omit</param>
        /// <param name="ifEmpty">Value to return if there are no categories</param>
        /// <returns>List of the game's categories, separated by commas.</returns>
        public string GetCatStringExcept(Category except, string ifEmpty = "")
        {
            string result = "";
            bool first = true;
            foreach (Category c in Categories)
            {
                if (c != except)
                {
                    if (!first)
                    {
                        result += ", ";
                    }
                    result += c.Name;
                    first = false;
                }
            }
            return first ? ifEmpty : result;
        }

        #endregion
    }

    /// <summary>
    ///     Represents a single game category
    /// </summary>
    public class Category : IComparable
    {
        public string Name;

        public Category(string name)
        {
            Name = name;
        }

        public int CompareTo(object o)
        {
            if (o == null) return 1;

            var otherCat = o as Category;
            if (o == null) throw new ArgumentException(GlobalStrings.Category_Exception_ObjectNotCategory);

            if (Name == otherCat.Name) return 0;

            if (Name == null) return -1;
            if (otherCat.Name == null) return 1;

            if (String.Equals(Name, "favorite", StringComparison.OrdinalIgnoreCase)) return -1;
            if (String.Equals(otherCat.Name, "favorite", StringComparison.OrdinalIgnoreCase))
                return 1;

            return String.CompareOrdinal(Name, otherCat.Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    ///     Represents a complete collection of games and categories.
    /// </summary>
    public class GameList
    {
        #region Fields

        private static readonly Regex rxUnicode = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);
        private readonly Category favoriteCategory;
        public List<Category> Categories;
        public Dictionary<int, GameInfo> Games;

        public Category FavoriteCategory
        {
            get { return favoriteCategory; }
        }

        #endregion

        public GameList()
        {
            Games = new Dictionary<int, GameInfo>();
            Categories = new List<Category>();
            favoriteCategory = new Category("favorite");
            Categories.Add(favoriteCategory);
        }

        #region Category management

        /// <summary>
        ///     Checks to see if a category with the given name exists
        /// </summary>
        /// <param name="name">Name of the category to look for</param>
        /// <returns>True if the name is found, false otherwise</returns>
        public bool CategoryExists(string name)
        {
            return Categories.Any(c => String.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Gets the category with the given name. If the category does not exist, creates it.
        /// </summary>
        /// <param name="name">Name to get the category for</param>
        /// <returns>A category with the given name. Null if any error is encountered.</returns>
        public Category GetCategory(string name)
        {
            // Categories must have a name
            if (string.IsNullOrEmpty(name)) return null;
            // Look for a matching category in the list and return if found
            foreach (Category c in Categories)
            {
                if (String.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)) return c;
            }
            // Create a new category and return it
            var newCat = new Category(name);
            Categories.Add(newCat);
            return newCat;
        }

        /// <summary>
        ///     Adds a new category to the list.
        /// </summary>
        /// <param name="name">Name of the category to add</param>
        /// <returns>The added category. Returns null if the category already exists.</returns>
        public Category AddCategory(string name)
        {
            if (string.IsNullOrEmpty(name) || CategoryExists(name))
            {
                return null;
            }
            var newCat = new Category(name);
            Categories.Add(newCat);
            return newCat;
        }

        /// <summary>
        ///     Removes the given category.
        /// </summary>
        /// <param name="c">Category to remove.</param>
        /// <returns>True if removal was successful, false if it was not in the list anyway</returns>
        public bool RemoveCategory(Category c)
        {
            // Can't remove favorite category
            if (c.Name == "favorite") return false;

            if (Categories.Remove(c))
            {
                foreach (GameInfo g in Games.Values)
                {
                    g.RemoveCategory(c);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Renames the given category.
        /// </summary>
        /// <param name="c">Category to rename.</param>
        /// <param name="newName">Name to assign to the new category.</param>
        /// <returns>True if rename was successful, false otherwise (if name was in use already)</returns>
        public bool RenameCategory(Category c, string newName)
        {
            if (c == favoriteCategory) return false;
            if (!CategoryExists(newName))
            {
                c.Name = newName;
                Categories.Sort();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Remove all empty categories from the category list.
        /// </summary>
        /// <returns>Number of categories removed</returns>
        public int RemoveEmptyCategories()
        {
            Dictionary<Category, int> counts = Categories.Where(c => c != favoriteCategory).ToDictionary(c => c, c => 0);
            foreach (GameInfo g in Games.Values)
            {
                foreach (Category c in g.Categories)
                {
                    if (counts.ContainsKey(c)) counts[c]++;
                }
            }
            return counts.Where(pair => pair.Value == 0).Count(pair => Categories.Remove(pair.Key));
        }

        #endregion

        #region General Modifiers

        public void Clear()
        {
            Games.Clear();
            Categories.Clear();
        }

        /// <summary>
        ///     Sets the name of the given game ID, and adds the game to the list if it doesn't already exist.
        /// </summary>
        /// <param name="id">ID of the game to set</param>
        /// <param name="name">Name to assign to the game</param>
        /// <param name="overWrite"></param>
        /// <returns>True if game was not already in the list, false otherwise</returns>
        private bool SetGameName(int id, string name, bool overWrite)
        {
            if (!Games.ContainsKey(id))
            {
                Games.Add(id, new GameInfo(id, name));
                return true;
            }
            if (overWrite)
            {
                Games[id].Name = name;
            }
            return false;
        }

        public void SetGameCategories(int gameID, Category cat, bool preserveFavorites)
        {
            SetGameCategories(gameID, new List<Category> {cat}, preserveFavorites);
        }

        public void SetGameCategories(int[] gameIDs, Category cat, bool preserveFavorites)
        {
            SetGameCategories(gameIDs, new List<Category> {cat}, preserveFavorites);
        }

        /// <summary>
        ///     Sets a game's categories to a particular set
        /// </summary>
        /// <param name="gameID">Game ID to modify</param>
        /// <param name="catSet">Set of categories to apply</param>
        /// <param name="preserveFavorites">If true, will not remove "favorite" category</param>
        public void SetGameCategories(int gameID, ICollection<Category> catSet, bool preserveFavorites)
        {
            GameInfo g = Games[gameID];
            bool reAddFav = preserveFavorites && g.ContainsCategory(favoriteCategory);
            g.SetCategories(catSet);
            if (reAddFav) g.AddCategory(favoriteCategory);
        }

        /// <summary>
        ///     Sets multiple games' categories to a particular set
        /// </summary>
        /// <param name="gameIDs"></param>
        /// <param name="catSet">Set of categories to apply</param>
        /// <param name="preserveFavorites">If true, will not remove "favorite" category</param>
        public void SetGameCategories(int[] gameIDs, ICollection<Category> catSet, bool preserveFavorites)
        {
            foreach (int t in gameIDs)
            {
                SetGameCategories(t, catSet, preserveFavorites);
            }
        }

        /// <summary>
        ///     Adds a single category to a single game
        /// </summary>
        /// <param name="gameID">Game ID to add category to</param>
        /// <param name="c">Category to add</param>
        public void AddGameCategory(int gameID, Category c)
        {
            GameInfo g = Games[gameID];
            g.AddCategory(c);
        }

        /// <summary>
        ///     Adds a single category to each member of a list of games
        /// </summary>
        /// <param name="gameIDs">List of game IDs to add to</param>
        /// <param name="c">Category to add</param>
        public void AddGameCategory(int[] gameIDs, Category c)
        {
            foreach (int t in gameIDs)
            {
                AddGameCategory(t, c);
            }
        }

        /// <summary>
        ///     Adds a set of categories to a single game
        /// </summary>
        /// <param name="gameID">Game ID to add to</param>
        /// <param name="cats">Categories to add</param>
        public void AddGameCategory(int gameID, ICollection<Category> cats)
        {
            GameInfo g = Games[gameID];
            g.AddCategory(cats);
        }

        /// <summary>
        ///     Adds a set of game categories to each member of a list of games
        /// </summary>
        /// <param name="gameIDs">List of game IDs to add to</param>
        /// <param name="cats">Categories to add</param>
        public void AddGameCategory(int[] gameIDs, ICollection<Category> cats)
        {
            foreach (int t in gameIDs)
            {
                AddGameCategory(t, cats);
            }
        }

        /// <summary>
        ///     Removes a single category from a single game.
        /// </summary>
        /// <param name="gameID">Game ID to remove from</param>
        /// <param name="c">Category to remove</param>
        public void RemoveGameCategory(int gameID, Category c)
        {
            GameInfo g = Games[gameID];
            g.RemoveCategory(c);
        }

        /// <summary>
        ///     Removes a single category from each member of a list of games
        /// </summary>
        /// <param name="gameIDs">List of game IDs to remove from</param>
        /// <param name="c">Category to remove</param>
        public void RemoveGameCategory(int[] gameIDs, Category c)
        {
            foreach (int t in gameIDs)
            {
                RemoveGameCategory(t, c);
            }
        }

        /// <summary>
        ///     Removes a set of categories from a single game
        /// </summary>
        /// <param name="gameID">Game ID to remove from</param>
        /// <param name="cats">Set of categories to remove</param>
        public void RemoveGameCategory(int gameID, ICollection<Category> cats)
        {
            GameInfo g = Games[gameID];
            g.RemoveCategory(cats);
        }

        /// <summary>
        ///     Removes a set of categories from a set of games
        /// </summary>
        /// <param name="gameIDs">List of game IDs to remove from</param>
        /// <param name="cats">Set of categories to remove</param>
        public void RemoveGameCategory(int[] gameIDs, ICollection<Category> cats)
        {
            for (int i = 0; i < gameIDs.Length; i++)
            {
                RemoveGameCategory(i, cats);
            }
        }

        public void ClearGameCategories(int gameID, bool preserveFavorite)
        {
            GameInfo g = Games[gameID];
            bool addFav = preserveFavorite && g.ContainsCategory(FavoriteCategory);
            g.ClearCategories();
            if (addFav) g.AddCategory(FavoriteCategory);
        }

        public void ClearGameCategories(int[] gameIDs, bool preserveFavorite)
        {
            foreach (int t in gameIDs)
            {
                ClearGameCategories(t, preserveFavorite);
            }
        }

        #endregion

        #region Profile Data Fetching

        /// <summary>
        ///     Grabs the XML game list for the given account and reads it into an XmlDocument.
        /// </summary>
        /// <param name="customUrl">The custom name for the account</param>
        /// <returns>Fetched XML page as an XmlDocument</returns>
        public static XmlDocument FetchXmlGameList(string customUrl)
        {
            return FetchXmlFromUrl(string.Format(Resources.UrlCustomGameListXml, customUrl));
        }

        /// <summary>
        ///     Grabs the XML game list for the given account and reads it into an XmlDocument.
        /// </summary>
        /// <returns>Fetched XML page as an XmlDocument</returns>
        public static XmlDocument FetchXmlGameList(Int64 steamId)
        {
            return FetchXmlFromUrl(string.Format(Resources.UrlGameListXml, steamId));
        }

        /// <summary>
        ///     Fetches an XML game list and loads it into an XML document.
        /// </summary>
        /// <param name="url">The URL to fetch</param>
        /// <returns>Fetched XML page as an XmlDocument</returns>
        public static XmlDocument FetchXmlFromUrl(string url)
        {
            var doc = new XmlDocument();
            try
            {
                Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_AttemptingDownloadXMLGameList, url);
                WebRequest req = WebRequest.Create(url);
                WebResponse response = req.GetResponse();
                if (response.ResponseUri.Segments.Length < 4)
                {
                    throw new ProfileAccessException(GlobalStrings.GameData_SpecifiedProfileNotPublic);
                }
                doc.Load(response.GetResponseStream());
                response.Close();
                Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_SuccessDownloadXMLGameList, url);
                return doc;
            }
            catch (ProfileAccessException)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ProfileNotPublic);
                throw;
            }
            catch (Exception e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ExceptionDownloadXMLGameList, e.Message);
                throw new ApplicationException(e.Message, e);
            }
        }

        /// <summary>
        ///     Grabs the HTML game list for the given account and returns its full text.
        /// </summary>
        /// <param name="customUrl">The custom name for the account</param>
        /// <returns>Full text of the HTTP response</returns>
        public static string FetchHtmlGameList(string customUrl)
        {
            return FetchHtmlFromUrl(string.Format(Resources.UrlCustomGameListHtml, customUrl));
        }

        /// <summary>
        ///     Grabs the HTML game list for the given account and returns its full text.
        /// </summary>
        /// <param name="accountId">The 64-bit account ID</param>
        /// <returns>Full text of the HTTP response</returns>
        public static string FetchHtmlGameList(Int64 accountId)
        {
            return FetchHtmlFromUrl(string.Format(Resources.UrlGameListHtml, accountId));
        }

        /// <summary>
        ///     Fetches an HTML game list and returns the full page text.
        ///     Mostly just grabs the given HTTP response, except that it throws an errors if the profile is not public, and writes
        ///     approrpriate log entries.
        /// </summary>
        /// <param name="url">The URL to fetch</param>
        /// <returns>The full text of the HTML page</returns>
        public static string FetchHtmlFromUrl(string url)
        {
            try
            {
                string result;

                Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_AttemptingDownloadHTMLGameList, url);
                WebRequest req = WebRequest.Create(url);
                using (WebResponse response = req.GetResponse())
                {
                    if (response.ResponseUri.Segments.Length < 4)
                    {
                        throw new ProfileAccessException(GlobalStrings.GameData_SpecifiedProfileNotPublic);
                    }
                    var sr = new StreamReader(response.GetResponseStream());
                    result = sr.ReadToEnd();
                }
                Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_SuccessDownloadHTMLGameList, url);
                return result;
            }
            catch (ProfileAccessException)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ProfileNotPublic);
                throw;
            }
            catch (Exception e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ExceptionDownloadHTMLGameList, e.Message);
                throw new ApplicationException(e.Message, e);
            }
        }

        #endregion

        #region Profile Data Integrating

        /// <summary>
        ///     Integrates list of games from an XmlDocument into the loaded game list.
        /// </summary>
        /// <param name="doc">The XmlDocument containing the new game list</param>
        /// <param name="overWrite">If true, overwrite the names of games already in the list.</param>
        /// <param name="ignore">A set of item IDs to ignore.</param>
        /// <param name="ignoreDlc">Ignore any items classified as DLC in the database.</param>
        /// <param name="newItems">The number of new items actually added</param>
        /// <returns>Returns the number of games successfully processed and not ignored.</returns>
        public int IntegrateXmlGameList(XmlDocument doc, bool overWrite, SortedSet<int> ignore, bool ignoreDlc,
            out int newItems)
        {
            newItems = 0;
            if (doc == null) return 0;
            int loadedGames = 0;
            XmlNodeList gameNodes = doc.SelectNodes("/gamesList/games/game");
            if (gameNodes != null)
                foreach (XmlNode gameNode in gameNodes)
                {
                    int appId;
                    XmlNode appIdNode = gameNode["appID"];
                    if (appIdNode != null && int.TryParse(appIdNode.InnerText, out appId))
                    {
                        XmlNode nameNode = gameNode["name"];
                        if (nameNode != null)
                        {
                            bool isNew;
                            bool added = IntegrateGame(appId, nameNode.InnerText, overWrite, ignore, ignoreDlc,
                                out isNew);
                            if (added)
                            {
                                loadedGames++;
                                if (isNew)
                                {
                                    newItems++;
                                }
                            }
                        }
                    }
                }
            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_IntegratedXMLDataIntoGameList, loadedGames,
                newItems);
            return loadedGames;
        }

        /// <summary>
        ///     Integrates list of games from an HTML page into the loaded game list.
        /// </summary>
        /// <param name="page">The full text of the page to load</param>
        /// <param name="overWrite">If true, overwrite the names of games already in the list.</param>
        /// <param name="ignore">A set of item IDs to ignore. Can be null.</param>
        /// <param name="ignoreDlc">Ignore any items classified as DLC in the database.</param>
        /// <param name="newItems">The number of new items actually added</param>
        /// <returns>Returns the number of games successfully processed and not ignored.</returns>
        public int IntegrateHtmlGameList(string page, bool overWrite, SortedSet<int> ignore, bool ignoreDlc,
            out int newItems)
        {
            newItems = 0;
            int totalItems = 0;

            var srch = new Regex("\"appid\":([0-9]+),\"name\":\"([^\"]+)\"");
            MatchCollection matches = srch.Matches(page);
            foreach (Match m in matches)
            {
                if (m.Groups.Count < 3) continue;
                string appIdString = m.Groups[1].Value;
                string appName = m.Groups[2].Value;

                int appId;
                if (int.TryParse(appIdString, out appId))
                {
                    appName = ProcessUnicode(appName);
                    bool isNew;
                    bool added = IntegrateGame(appId, appName, overWrite, ignore, ignoreDlc, out isNew);
                    if (added)
                    {
                        totalItems++;
                        if (isNew)
                        {
                            newItems++;
                        }
                    }
                }
            }
            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_IntegratedHTMLDataIntoGameList, totalItems,
                newItems);
            return totalItems;
        }

        /// <summary>
        ///     Searches a string for HTML unicode entities ('\u####') and replaces them with actual unicode characters.
        /// </summary>
        /// <param name="val">The string to process</param>
        /// <returns>The processed string</returns>
        public string ProcessUnicode(string val)
        {
            return rxUnicode.Replace(
                val,
                m => ((char) int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
                );
        }

        /// <summary>
        ///     Loads in games from a VDF node containing a list of games.
        ///     Any games in the node not found in the game list will be added to the gamelist.
        ///     If a game in the node has a tags subnode, the "favorite" field will be overwritten.
        ///     If a game in the node has a category set, it will overwrite any categories in the gamelist.
        ///     If a game in the node does NOT have a category set, the category in the gamelist will NOT be cleared.
        /// </summary>
        /// <param name="appsNode">Node containing the game nodes</param>
        /// <param name="ignore">Set of games to ignore</param>
        /// <param name="ignoreDlc"></param>
        /// <returns>Number of games loaded</returns>
        private int IntegrateGamesFromVdf(VdfFileNode appsNode, SortedSet<int> ignore, bool ignoreDlc)
        {
            int loadedGames = 0;

            Dictionary<string, VdfFileNode> gameNodeArray = appsNode.NodeArray;
            if (gameNodeArray != null)
            {
                foreach (var gameNodePair in gameNodeArray)
                {
                    int gameId;
                    if (int.TryParse(gameNodePair.Key, out gameId))
                    {
                        if ((ignore != null && ignore.Contains(gameId)) || (ignoreDlc && Program.GameDB.IsDlc(gameId)))
                        {
                            Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_SkippedProcessingGame,
                                gameId);
                            continue;
                        }
                        if (gameNodePair.Value != null && gameNodePair.Value.ContainsKey("tags"))
                        {
                            var cats = new SortedSet<Category>();

                            loadedGames++;

                            VdfFileNode tagsNode = gameNodePair.Value["tags"];
                            Dictionary<string, VdfFileNode> tagArray = tagsNode.NodeArray;
                            if (tagArray != null)
                            {
                                foreach (VdfFileNode tag in tagArray.Values)
                                {
                                    string tagName = tag.NodeString;
                                    if (tagName != null)
                                    {
                                        Category c = GetCategory(tagName);
                                        if (c != null) cats.Add(c);
                                    }
                                }
                            }

                            bool hidden = false;
                            if (gameNodePair.Value.ContainsKey("hidden"))
                            {
                                VdfFileNode hiddenNode = gameNodePair.Value["hidden"];
                                hidden = (hiddenNode.NodeString == "1" || hiddenNode.NodeInt == 1);
                            }


                            // Add the game to the list if it doesn't exist already
                            if (!Games.ContainsKey(gameId))
                            {
                                var newGame = new GameInfo(gameId, string.Empty);
                                Games.Add(gameId, newGame);
                                newGame.Name = Program.GameDB.GetName(gameId);
                                Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_AddedNewGame, gameId,
                                    newGame.Name);
                            }

                            if (cats.Count > 0)
                            {
                                SetGameCategories(gameId, cats, false);
                            }
                            Games[gameId].Hidden = hidden;

                            //TODO: Don't think SortedSet.ToString() does what I hope
                            Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_ProcessedGame, gameId,
                                (cats.Count == 0) ? "~" : cats.ToString());
                        }
                    }
                }
            }

            return loadedGames;
        }

        /// <summary>
        ///     Adds a new game to the database, or updates an existing game with new information.
        /// </summary>
        /// <param name="appId">App ID to add or update</param>
        /// <param name="appName">Name of app to add, or update to</param>
        /// <param name="overWrite">If true, will overwrite any existing games. If false, will fail if the game already exists.</param>
        /// <param name="ignore">Set of games to ignore. Can be null. If the game is in this list, no action will be taken.</param>
        /// <param name="ignoreDlc">If true, ignore the game if it is marked as DLC in the loaded database.</param>
        /// <param name="isNew">If true, a new game was added. If false, an existing game was updated, or the operation failed.</param>
        /// <returns>True if the game was integrated, false otherwise.</returns>
        private bool IntegrateGame(int appId, string appName, bool overWrite, SortedSet<int> ignore, bool ignoreDlc,
            out bool isNew)
        {
            isNew = false;
            if ((ignore != null && ignore.Contains(appId)) || (ignoreDlc && Program.GameDB.IsDlc(appId)))
            {
                Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_SkippedIntegratingGame, appId, appName);
                return false;
            }
            isNew = SetGameName(appId, appName, overWrite);
            Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_IntegratedGameIntoGameList, appId, appName,
                isNew);
            return true;
        }

        #endregion

        #region Steam config file handling

        /// <summary>
        ///     Loads category info from the given steam config file.
        /// </summary>
        /// <param name="filePath">The path of the file to open</param>
        /// <param name="ignore"></param>
        /// <param name="ignoreDlc"></param>
        /// <returns>The number of game entries found</returns>
        public int ImportSteamConfigFile(string filePath, SortedSet<int> ignore, bool ignoreDlc)
        {
            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_OpeningSteamConfigFile, filePath);
            TextVdfFileNode dataRoot;

            try
            {
                using (var reader = new StreamReader(filePath, false))
                {
                    dataRoot = TextVdfFileNode.Load(reader, true);
                }
            }
            catch (ParseException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorParsingConfigFileParam, e.Message);
                throw new ApplicationException(GlobalStrings.GameData_ErrorParsingSteamConfigFile + e.Message, e);
            }
            catch (IOException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.Message);
                throw new ApplicationException(GlobalStrings.GameData_ErrorOpeningSteamConfigFile + e.Message, e);
            }

            VdfFileNode appsNode = dataRoot.GetNodeAt(new[] {"Software", "Valve", "Steam", "apps"});
            int count = IntegrateGamesFromVdf(appsNode, ignore, ignoreDlc);
            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_SteamConfigFileLoaded, count);
            return count;
        }

        /// <summary>
        ///     Loads category info from the steam config file for the given Steam user.
        /// </summary>
        /// <param name="SteamId">Identifier of Steam user</param>
        /// <param name="ignore">Set of games to ignore</param>
        /// <param name="ignoreDlc">If true, ignore games marked as DLC in the database</param>
        /// <param name="ignoreExternal">If false, also load non-steam games</param>
        /// <returns>The number of game entries found</returns>
        public int ImportSteamConfigFile(long SteamId, SortedSet<int> ignore, bool ignoreDlc, bool ignoreExternal)
        {
            string filePath = string.Format(Resources.ConfigFilePath, Settings.Instance().SteamPath,
                Profile.ID64toDirName(SteamId));
            int result = ImportSteamConfigFile(filePath, ignore, ignoreDlc);
            if (!ignoreExternal)
            {
                int newItems, removedItems;
                result += ImportSteamShortcuts(SteamId, true, out newItems, out removedItems);
            }
            return result;
        }

        /// <summary>
        ///     Writes Steam game category information to Steam user config file.
        /// </summary>
        /// <param name="SteamId">Steam ID of user to save the config file for</param>
        /// <param name="discardMissing">
        ///     If true, any pre-existing game entries in the file that do not have corresponding entries
        ///     in the GameList are removed
        /// </param>
        public void ExportSteamConfigFile(long SteamId, bool discardMissing)
        {
            string filePath = string.Format(Resources.ConfigFilePath, Settings.Instance().SteamPath,
                Profile.ID64toDirName(SteamId));
            ExportSteamConfigFile(filePath, discardMissing);
        }

        /// <summary>
        ///     Writes Steam game category information to Steam user config file.
        /// </summary>
        /// <param name="filePath">Full path of the steam config file to save</param>
        /// <param name="discardMissing">
        ///     If true, any pre-existing game entries in the file that do not have corresponding entries
        ///     in the GameList are removed
        /// </param>
        public void ExportSteamConfigFile(string filePath, bool discardMissing)
        {
            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_SavingSteamConfigFile, filePath);

            var fileData = new TextVdfFileNode();
            try
            {
                using (var reader = new StreamReader(filePath, false))
                {
                    fileData = TextVdfFileNode.Load(reader, true);
                }
            }
            catch (Exception e)
            {
                Program.Logger.Write(LoggerLevel.Warning, GlobalStrings.GameData_LoadingErrorSteamConfig, e.Message);
            }

            VdfFileNode appListNode = fileData.GetNodeAt(new[] {"Software", "Valve", "Steam", "apps"});

            // Run through all Delete category data for any games not found in the GameList
            if (discardMissing)
            {
                Dictionary<string, VdfFileNode> gameNodeArray = appListNode.NodeArray;
                if (gameNodeArray != null)
                {
                    foreach (var pair in gameNodeArray)
                    {
                        int gameId;
                        if (!(int.TryParse(pair.Key, out gameId) && Games.ContainsKey(gameId)))
                        {
                            Program.Logger.Write(LoggerLevel.Verbose,
                                GlobalStrings.GameData_RemovingGameCategoryFromSteamConfig, gameId);
                            pair.Value.RemoveSubnode("tags");
                        }
                    }
                }
            }

            // Force appListNode to be an array, we can't do anything if it's a value
            appListNode.MakeArray();

            foreach (GameInfo game in Games.Values)
            {
                if (game.Id > 0)
                {
                    // External games have negative identifier
                    Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_AddingGameToConfigFile, game.Id);
                    VdfFileNode gameNode = appListNode[game.Id.ToString()];
                    gameNode.MakeArray();

                    VdfFileNode tagsNode = gameNode["tags"];
                    tagsNode.MakeArray();

                    Dictionary<string, VdfFileNode> tags = tagsNode.NodeArray;
                    if (tags != null) tags.Clear();

                    int key = 0;
                    foreach (Category c in game.Categories)
                    {
                        tagsNode[key.ToString()] = new TextVdfFileNode(c.Name);
                        key++;
                    }

                    if (game.Hidden)
                    {
                        gameNode["hidden"] = new TextVdfFileNode("1");
                    }
                    else
                    {
                        gameNode.RemoveSubnode("hidden");
                    }
                }
            }


            Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_CleaningUpSteamConfigTree);
            appListNode.CleanTree();

            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_WritingToDisk);
            var fullFile = new TextVdfFileNode();
            fullFile["UserLocalConfigStore"] = fileData;
            try
            {
                Utility.BackupFile(filePath, Settings.Instance().ConfigBackupCount);
            }
            catch (Exception e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.Log_GameData_ConfigBackupFailed, e.Message);
            }
            try
            {
                string filePathTmp = filePath + ".tmp";
                var f = new FileInfo(filePathTmp);
                if (f.Directory != null) f.Directory.Create();
                FileStream fStream = f.Open(FileMode.Create, FileAccess.Write, FileShare.None);
                using (var writer = new StreamWriter(fStream))
                {
                    fullFile.Save(writer);
                }
                fStream.Close();
                File.Delete(filePath);
                File.Move(filePathTmp, filePath);
            }
            catch (ArgumentException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());
                throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigBadPath, e);
            }
            catch (IOException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());
                throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigFile + e.Message, e);
            }
            catch (UnauthorizedAccessException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorSavingSteamConfigFile, e.ToString());
                throw new ApplicationException(GlobalStrings.GameData_AccessDeniedSteamConfigFile + e.Message, e);
            }
        }

        #endregion

        #region Non-Steam game file handling

        /// <summary>
        ///     Writes category info for shortcut games to shortcuts.vdf config file for specified Steam user.
        ///     Loads the shortcut config file, then tries to match each game in the file against one of the games in the gamelist.
        ///     If it finds a match, it updates the config file with the new category info.
        /// </summary>
        /// <param name="SteamId">Identifier of Steam user to save information</param>
        public void ExportSteamShortcuts(long SteamId)
        {
            string filePath = string.Format(Resources.ShortCutsFilePath, Settings.Instance().SteamPath,
                Profile.ID64toDirName(SteamId));
            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_SavingSteamConfigFile, filePath);
            FileStream fStream = null;
            BinaryReader binReader = null;
            BinaryVdfFileNode dataRoot = null;
            try
            {
                fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                binReader = new BinaryReader(fStream);

                dataRoot = BinaryVdfFileNode.Load(binReader);
            }
            catch (FileNotFoundException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.ToString());
            }
            catch (IOException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_LoadingErrorSteamConfig, e.ToString());
            }
            if (binReader != null)
                binReader.Close();
            if (fStream != null)
                fStream.Close();
            if (dataRoot != null)
            {
                List<GameInfo> gamesToSave = (from id in Games.Keys where id < 0 select Games[id]).ToList();

                StringDictionary launchIds;
                LoadShortcutLaunchIds(SteamId, out launchIds);

                VdfFileNode appsNode = dataRoot.GetNodeAt(new[] {"shortcuts"}, false);
                foreach (var shortcutPair in appsNode.NodeArray)
                {
                    VdfFileNode nodeGame = shortcutPair.Value;
                    int nodeId;
                    int.TryParse(shortcutPair.Key, out nodeId);

                    int matchingIndex = FindMatchingShortcut(nodeId, nodeGame, gamesToSave, launchIds);

                    if (matchingIndex >= 0)
                    {
                        GameInfo game = gamesToSave[matchingIndex];
                        gamesToSave.RemoveAt(matchingIndex);

                        Program.Logger.Write(LoggerLevel.Verbose, GlobalStrings.GameData_AddingGameToConfigFile, game.Id);

                        VdfFileNode tagsNode = nodeGame.GetNodeAt(new[] {"tags"});
                        Dictionary<string, VdfFileNode> tags = tagsNode.NodeArray;
                        if (tags != null)
                        {
                            tags.Clear();
                        }

                        int index = 0;
                        foreach (Category c in game.Categories)
                        {
                            tagsNode[index.ToString()] = new BinaryVdfFileNode(c.Name);
                            index++;
                        }

                        nodeGame["hidden"] = new BinaryVdfFileNode(game.Hidden ? 1 : 0);
                    }
                }
                if (dataRoot.NodeType == ValueType.Array)
                {
                    Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_SavingShortcutConfigFile, filePath);
                    try
                    {
                        Utility.BackupFile(filePath, Settings.Instance().ConfigBackupCount);
                    }
                    catch (Exception e)
                    {
                        Program.Logger.Write(LoggerLevel.Error, GlobalStrings.Log_GameData_ShortcutBackupFailed,
                            e.Message);
                    }
                    try
                    {
                        string filePathTmp = filePath + ".tmp";
                        fStream = new FileStream(filePathTmp, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        var binWriter = new BinaryWriter(fStream);
                        dataRoot.Save(binWriter);
                        binWriter.Close();
                        fStream.Close();
                        File.Delete(filePath);
                        File.Move(filePathTmp, filePath);
                    }
                    catch (ArgumentException e)
                    {
                        Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorSavingSteamConfigFile,
                            e.ToString());
                        throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigBadPath, e);
                    }
                    catch (IOException e)
                    {
                        Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorSavingSteamConfigFile,
                            e.ToString());
                        throw new ApplicationException(GlobalStrings.GameData_FailedToSaveSteamConfigFile + e.Message, e);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorSavingSteamConfigFile,
                            e.ToString());
                        throw new ApplicationException(GlobalStrings.GameData_AccessDeniedSteamConfigFile + e.Message, e);
                    }
                }
            }
        }

        /// <summary>
        ///     Load launch IDs for external games from screenshots.vdf
        /// </summary>
        /// <param name="SteamId">Steam user identifier</param>
        /// <param name="shortcutLaunchIds">Found games listed as pairs of {gameName, gameId} </param>
        /// <returns>True if file was successfully loaded, false otherwise</returns>
        private bool LoadShortcutLaunchIds(long SteamId, out StringDictionary shortcutLaunchIds)
        {
            bool result = false;
            string filePath = string.Format(Resources.ScreenshotsFilePath, Settings.Instance().SteamPath,
                Profile.ID64toDirName(SteamId));

            shortcutLaunchIds = new StringDictionary();

            StreamReader reader = null;
            try
            {
                reader = new StreamReader(filePath, false);
                TextVdfFileNode dataRoot = TextVdfFileNode.Load(reader, true);

                VdfFileNode appsNode = dataRoot.GetNodeAt(new[] {"shortcutnames"}, false);

                foreach (var shortcutPair in appsNode.NodeArray)
                {
                    string launchId = shortcutPair.Key;
                    var gameName = (string) shortcutPair.Value.NodeData;
                    if (!shortcutLaunchIds.ContainsKey(gameName))
                    {
                        shortcutLaunchIds.Add(gameName, launchId);
                    }
                }
                result = true;
            }
            catch (FileNotFoundException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.ToString());
            }
            catch (IOException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_LoadingErrorSteamConfig, e.ToString());
            }

            if (reader != null)
            {
                reader.Close();
            }

            return result;
        }

        /// <summary>
        ///     Updates set of non-Steam games. Will remove any games that are currently in the list but not found in the Steam
        ///     config.
        /// </summary>
        /// <param name="SteamId">The ID64 of the account to load shortcuts for</param>
        /// <param name="overwriteCategories">
        ///     If true, overwrite categories for found games. If false, only load categories for
        ///     games without a category already set.
        /// </param>
        /// <param name="newItems">Number of new items added</param>
        /// <param name="removedItems">Number of old items removed.</param>
        /// <returns>Total number of entries processed</returns>
        public int ImportSteamShortcuts(long SteamId, bool overwriteCategories, out int newItems, out int removedItems)
        {
            newItems = 0;
            removedItems = 0;
            if (SteamId <= 0) return 0;
            int loadedGames = 0;

            string filePath = string.Format(Resources.ShortCutsFilePath, Settings.Instance().SteamPath,
                Profile.ID64toDirName(SteamId));
            FileStream fStream = null;
            BinaryReader binReader = null;
            try
            {
                fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                binReader = new BinaryReader(fStream);

                BinaryVdfFileNode dataRoot = BinaryVdfFileNode.Load(binReader);

                VdfFileNode shortcutsNode = dataRoot.GetNodeAt(new[] {"shortcuts"}, false);

                if (shortcutsNode != null)
                {
                    List<GameInfo> oldShortcutGames = (from id in Games.Keys where id < 0 select Games[id]).ToList();
                    foreach (GameInfo g in oldShortcutGames)
                    {
                        Games.Remove(g.Id);
                    }

                    StringDictionary launchIds;
                    LoadShortcutLaunchIds(SteamId, out launchIds);

                    foreach (var shortcutPair in shortcutsNode.NodeArray)
                    {
                        VdfFileNode nodeGame = shortcutPair.Value;
                        nodeGame.GetNodeAt(new[] {"appname"}, false);

                        int gameId;
                        int.TryParse(shortcutPair.Key, out gameId);

                        bool success = IntegrateShortcut(gameId, nodeGame, oldShortcutGames, launchIds, ref newItems);
                        if (success) loadedGames++;
                    }
                    removedItems = oldShortcutGames.Count;
                }
            }
            catch (FileNotFoundException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_ErrorOpeningConfigFileParam, e.ToString());
            }
            catch (IOException e)
            {
                Program.Logger.Write(LoggerLevel.Error, GlobalStrings.GameData_LoadingErrorSteamConfig, e.ToString());
            }
            catch (ParseException e)
            {
                Program.Logger.Write(LoggerLevel.Error, e.ToString());
            }
            finally
            {
                if (binReader != null)
                {
                    binReader.Close();
                }
                if (fStream != null)
                {
                    fStream.Close();
                }
            }

            Program.Logger.Write(LoggerLevel.Info, GlobalStrings.GameData_IntegratedShortCuts, loadedGames, newItems,
                removedItems);

            return loadedGames;
        }

        /// <summary>
        ///     Searches a list of games, looking for the one that matches the information in the shortcut node.
        ///     Checks launch ID first, then checks a combination of name and ID, then just checks name.
        /// </summary>
        /// <param name="shortcutId">ID of the shortcut node</param>
        /// <param name="shortcutNode">Shotcut node itself</param>
        /// <param name="gamesToMatchAgainst">List of game objects to match against</param>
        /// <param name="shortcutLaunchIds">List of launch IDs referenced by name</param>
        /// <returns>The index of the matching game if found, -1 otherwise.</returns>
        private int FindMatchingShortcut(int shortcutId, VdfFileNode shortcutNode, List<GameInfo> gamesToMatchAgainst,
            StringDictionary shortcutLaunchIds)
        {
            VdfFileNode nodeName = shortcutNode.GetNodeAt(new[] {"appname"}, false);
            string gameName = (nodeName != null) ? nodeName.NodeString : null;
            string launchId = shortcutLaunchIds[gameName];
            // First, look for games with matching launch IDs.
            for (int i = 0; i < gamesToMatchAgainst.Count; i++)
            {
                if (gamesToMatchAgainst[i].LaunchString == launchId) return i;
            }
            // Second, look for games with matching names AND matching shortcut IDs.
            for (int i = 0; i < gamesToMatchAgainst.Count; i++)
            {
                if (gamesToMatchAgainst[i].Id == -(shortcutId + 1) && gamesToMatchAgainst[i].Name == gameName) return i;
            }
            // Third, just look for name matches
            for (int i = 0; i < gamesToMatchAgainst.Count; i++)
            {
                if (gamesToMatchAgainst[i].Name == gameName) return i;
            }

            return -1;
        }

        /// <summary>
        ///     Adds a non-steam game to the gamelist.
        /// </summary>
        /// <param name="gameId">ID of the game in the steam config file</param>
        /// <param name="gameNode">Node for the game in the steam config file</param>
        /// <param name="oldShortcuts">List of un-matched non-steam games from the gamelist before the update</param>
        /// <param name="launchIds">Dictionary of launch ids (name:launchId)</param>
        /// <param name="newGames">Number of NEW games that have been added to the list</param>
        /// <param name="preferSteamCategories">
        ///     If true, prefers to use the categories from the steam config if there is a
        ///     conflict. If false, prefers to use the categories from the existing gamelist.
        /// </param>
        /// <returns>True if the game was successfully added</returns>
        private bool IntegrateShortcut(int gameId, VdfFileNode gameNode, List<GameInfo> oldShortcuts,
            StringDictionary launchIds, ref int newGames, bool preferSteamCategories = true)
        {
            VdfFileNode nodeName = gameNode.GetNodeAt(new[] {"appname"}, false);
            string gameName = (nodeName != null) ? nodeName.NodeString : null;
            // The ID of the created game must be negative
            int newId = -(gameId + 1);

            // This should never happen, but just in case
            if (Games.ContainsKey(newId))
            {
                return false;
            }

            var game = new GameInfo(newId, gameName);
            Games.Add(newId, game);

            game.LaunchString = launchIds[gameName];

            int oldShortcutId = FindMatchingShortcut(gameId, gameNode, oldShortcuts, launchIds);
            bool oldCatSet = (oldShortcutId != -1) && oldShortcuts[oldShortcutId].Categories.Count > 0;
            if (oldShortcutId == -1) newGames++;

            VdfFileNode tagsNode = gameNode.GetNodeAt(new[] {"tags"}, false);
            bool steamCatSet = (tagsNode != null && tagsNode.NodeType == ValueType.Array && tagsNode.NodeArray.Count > 0);

            //fill in categories
            if (steamCatSet && (preferSteamCategories || !oldCatSet))
            {
                // Fill in categories from the Steam shortcut file
                foreach (var tag in tagsNode.NodeArray)
                {
                    string tagName = tag.Value.NodeString;
                    game.AddCategory(GetCategory(tagName));
                }
            }
            else if (oldShortcutId >= 0 && oldShortcutId < oldShortcuts.Count)
            {
                // Fill in categories from the game list
                game.SetCategories(oldShortcuts[oldShortcutId].Categories);
            }

            game.Hidden = false;
            if (gameNode.ContainsKey("hidden"))
            {
                VdfFileNode hiddenNode = gameNode["hidden"];
                game.Hidden = (hiddenNode.NodeString == "1" || hiddenNode.NodeInt == 1);
            }

            if (oldShortcutId != -1) oldShortcuts.RemoveAt(oldShortcutId);
            return true;
        }

        #endregion
    }

    internal class ProfileAccessException : ApplicationException
    {
        public ProfileAccessException(string m) : base(m)
        {
        }
    }
}