/*
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
using System.Xml;
using Depressurizer.Lib;

namespace Depressurizer
{
    internal class CDlgUpdateProfile : CancelableDlg
    {
        private readonly Int64 SteamId;
        private readonly bool custom;
        private readonly string customUrl;
        private readonly GameList data;

        private readonly SortedSet<int> ignore;
        private readonly bool ignoreDlc;

        //jpodadera. Non-Steam games
        private readonly bool ignoreExternal;
        private readonly bool overwrite;
        private XmlDocument doc;
        private string htmlDoc;

        public CDlgUpdateProfile(GameList data, Int64 accountId, bool overwrite, SortedSet<int> ignore, bool ignoreDlc,
            bool ignoreExternal)
            : base(GlobalStrings.CDlgUpdateProfile_UpdatingGameList, true)
        {
            custom = false;
            SteamId = accountId;

            Added = 0;
            Fetched = 0;
            UseHtml = false;
            Failover = false;

            this.data = data;

            this.overwrite = overwrite;
            this.ignore = ignore;
            this.ignoreDlc = ignoreDlc;

            //jpodadera. Non-Steam games
            this.ignoreExternal = ignoreExternal;

            SetText(GlobalStrings.CDlgFetch_DownloadingGameList);
        }

        public CDlgUpdateProfile(GameList data, string customUrl, bool overwrite, SortedSet<int> ignore, bool ignoreDlc,
            bool ignoreExternal)
            : base(GlobalStrings.CDlgUpdateProfile_UpdatingGameList, true)
        {
            custom = true;
            this.customUrl = customUrl;

            Added = 0;
            Fetched = 0;
            UseHtml = false;
            Failover = false;

            this.data = data;

            this.overwrite = overwrite;
            this.ignore = ignore;
            this.ignoreDlc = ignoreDlc;

            //jpodadera. Non-Steam games
            this.ignoreExternal = ignoreExternal;

            SetText(GlobalStrings.CDlgFetch_DownloadingGameList);
        }

        public int Fetched { get; private set; }
        public int Added { get; private set; }
        public int Removed { get; private set; }

        public bool UseHtml { get; private set; }
        public bool Failover { get; private set; }

        protected override void RunProcess()
        {
            Added = 0;
            Fetched = 0;
            switch (Settings.Instance().ListSource)
            {
                case GameListSource.XmlPreferred:
                    FetchXmlPref();
                    break;
                case GameListSource.XmlOnly:
                    FetchXml();
                    break;
                case GameListSource.WebsiteOnly:
                    FetchHtml();
                    break;
            }

            OnThreadCompletion();
        }

        protected void FetchXml()
        {
            UseHtml = false;
            doc = custom ? GameList.FetchXmlGameList(customUrl) : GameList.FetchXmlGameList(SteamId);
        }

        protected void FetchHtml()
        {
            UseHtml = true;
            htmlDoc = custom ? GameList.FetchHtmlGameList(customUrl) : GameList.FetchHtmlGameList(SteamId);
        }

        protected void FetchXmlPref()
        {
            try
            {
                FetchXml();
                return;
            }
            catch (ProfileAccessException)
            {
                throw;
            }
            catch
            {
            }
            Failover = true;
            FetchHtml();
        }

        protected override void Finish()
        {
            if (!Canceled && Error == null && (UseHtml ? (htmlDoc != null) : (doc != null)))
            {
                SetText(GlobalStrings.CDlgFetch_FinishingDownload);
                if (UseHtml)
                {
                    int newItems;
                    Fetched = data.IntegrateHtmlGameList(htmlDoc, overwrite, ignore, ignoreDlc, out newItems);
                    Added = newItems;
                }
                else
                {
                    int newItems;
                    Fetched = data.IntegrateXmlGameList(doc, overwrite, ignore, ignoreDlc, out newItems);
                    Added = newItems;
                }

                //jpodadera. Non-Steam games
                if ((!ignoreExternal) && (SteamId != 0))
                {
                    int newItems, removedItems;
                    Fetched += data.ImportSteamShortcuts(SteamId, overwrite, out newItems, out removedItems);
                    Added += newItems;
                    Removed = removedItems;
                }
                else
                    Removed = 0;

                OnJobCompletion();
            }
        }
    }
}