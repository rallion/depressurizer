using Depressurizer.SteamFileAccess.ApplicationManifest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depressurizer.Games
{
    public class InstalledGames
    {
        private readonly IReadOnlyDictionary<int, AppManifest> UnmodifiableGamesList;

        public InstalledGames()
        {
            UnmodifiableGamesList = new Dictionary<int, AppManifest>();
        }

        public InstalledGames(IReadOnlyDictionary<int, AppManifest> UnmodifiableGamesList)
        {
            this.UnmodifiableGamesList = UnmodifiableGamesList;
        }

        public IEnumerable<int> getInstalledGamesList()
        {
            return UnmodifiableGamesList.Keys;
        }

        public List<AppManifest> GetMatchingGames(int[] gameIds)
        {
            List<AppManifest> matchingGames = new List<AppManifest>();

            foreach(int gameId in gameIds)
            {
                AppManifest gameManifest = null;
                if (UnmodifiableGamesList.TryGetValue(gameId, out gameManifest))
                {
                    matchingGames.Add(gameManifest);
                }
            }

            return matchingGames;
        }
    }
}
