using System;
using System.Collections.Generic;
using System.Linq;

namespace MagmaMC.PSC
{
    /// <summary>
    /// Represents a group with its permissions and players.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        public Group()
        {
            Permissions = new List<string>();
            Players = new List<string>();
        }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets the permissions of the group.
        /// </summary>
        public List<string> Permissions
        {
            get; set;
        }

        /// <summary>
        /// Gets the players in the group.
        /// </summary>
        public List<string> Players
        {
            get; set;
        }
    }
    /// <summary>
    /// Represents a parser for Magma's Permission System Config data.
    /// </summary>
    public class PSC
    {
        /// <summary>
        /// Name, Permissions, Players
        /// </summary>
        private Dictionary<string, Group> _groups;
        private bool Initialized = false;

        public PSC() { }

        public PSC(string Data) => Load(Data, false);


        /// <summary>
        /// Loads the Permission System Config data.
        /// </summary>
        /// <param name="Data">The data to load.</param>
        public void Load(string Data) => Load(Data, false);

        /// <summary>
        /// Loads the Permission System Config data.
        /// </summary>
        /// <param name="Data">The data to load.</param>
        /// <param name="Overwrite">If set to <c>true</c>, overwrites existing data.</param>
        public void Load(string Data, bool Overwrite)
        {
            if (!Overwrite && Initialized)
                throw new AlreadyInitializedException();
            _groups = ParseData(Data);
            Initialized = true;
        }
        private Dictionary<string, Group> ParseData(string data)
        {
            var groups = new Dictionary<string, Group>();
            var lines = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Group currentGroup = null;
            foreach (var line in lines)
            {
                if (line.StartsWith(">>"))
                {
                    string GroupData = line.Substring(2).Trim();
                    string[] groupInfo = GroupData.Split('>');
                    string groupName = groupInfo[0].Trim();
                    currentGroup = new Group { Name = groupName };
                    currentGroup.Permissions.AddRange(groupInfo[1].Trim().Split('+'));
                    groups[groupName] = currentGroup;
                }
                else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//") && currentGroup != null)
                {
                    currentGroup.Players.Add(line.Trim());
                }
            }
            return groups;
        }


        /// <summary>
        /// Gets the list of groups.
        /// </summary>
        /// <returns>An array of group names.</returns>
        /// <exception cref="NotInitializedException">Thrown when the PSC instance is not initialized.</exception>
        public string[] GetGroups()
        {
            if (!Initialized)
                throw new NotInitializedException();

            return _groups.Keys.ToArray();
        }

        /// <summary>
        /// Gets the players with given permissions.
        /// </summary>
        /// <param name="Permissions">The permissions to filter by.</param>
        /// <returns>An array of player names.</returns>
        /// <exception cref="NotInitializedException">Thrown when the PSC instance is not initialized.</exception>
        public string[] GetPlayers(string[] Permissions)
        {
            if (!Initialized)
                throw new NotInitializedException();

            var players = new List<string>();
            foreach (var group in _groups.Values)
            {
                if (Permissions.Any(p => group.Permissions.Contains(p)))
                    players.AddRange(group.Players);
            }
            return players.Distinct().ToArray();
        }

        /// <summary>
        /// Gets the players in a specific group.
        /// </summary>
        /// <param name="GroupName">The name of the group.</param>
        /// <returns>An array of player names.</returns>
        /// <exception cref="NotInitializedException">Thrown when the PSC instance is not initialized.</exception>
        /// <exception cref="GroupNotFoundException">Thrown when the group is not found.</exception>
        public string[] GetPlayers(string GroupName)
        {
            if (!Initialized)
                throw new NotInitializedException();

            if (!_groups.ContainsKey(GroupName))
                throw new GroupNotFoundException();

            return _groups[GroupName].Players.ToArray();
        }

        /// <summary>
        /// Adds a new group with specified permissions.
        /// </summary>
        /// <param name="GroupName">The name of the group.</param>
        /// <param name="Permissions">The permissions of the group.</param>
        /// <exception cref="NotInitializedException">Thrown when the PSC instance is not initialized.</exception>
        /// <exception cref="GroupAlreadyExistsException">Thrown when the group already exists.</exception>
        public void AddGroup(string GroupName, string[] Permissions)
        {
            if (!Initialized)
                throw new NotInitializedException();

            if (_groups.ContainsKey(GroupName))
                throw new GroupAlreadyExistsException();

            var group = new Group { Name = GroupName, Permissions = Permissions.ToList() };
            _groups[GroupName] = group;

            foreach (var existingGroup in _groups.Values)
            {
                foreach (var permission in Permissions)
                {
                    if (existingGroup.Permissions.Contains(permission))
                        existingGroup.Players.AddRange(group.Players);
                }
            }
        }

        /// <summary>
        /// Removes a group.
        /// </summary>
        /// <param name="GroupName">The name of the group to remove.</param>
        /// <exception cref="NotInitializedException">Thrown when the PSC instance is not initialized.</exception>
        /// <exception cref="GroupNotFoundException">Thrown when the group is not found.</exception>
        public void RemoveGroup(string GroupName)
        {
            if (!Initialized)
                throw new NotInitializedException();

            if (!_groups.ContainsKey(GroupName))
                throw new GroupNotFoundException();

            _groups.Remove(GroupName);
        }

        /// <summary>
        /// Adds a player to a group.
        /// </summary>
        /// <param name="PlayerID">The ID of the player to add.</param>
        /// <param name="GroupName">The name of the group to add the player to.</param>
        /// <returns><c>true</c> if the player was added successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="NotInitializedException">Thrown when the PSC instance is not initialized.</exception>
        /// <exception cref="GroupNotFoundException">Thrown when the group is not found.</exception>
        public bool AddPlayer(string PlayerID, string GroupName)
        {
            if (!Initialized)
                throw new NotInitializedException();

            if (!_groups.ContainsKey(GroupName))
                throw new GroupNotFoundException();

            if (_groups[GroupName].Players.Contains(PlayerID))
                return false;

            _groups[GroupName].Players.Add(PlayerID);
            return true;
        }

        /// <summary>
        /// Removes a player from a group.
        /// </summary>
        /// <param name="PlayerID">The ID of the player to remove.</param>
        /// <param name="GroupName">The name of the group to remove the player from.</param>
        /// <exception cref="NotInitializedException">Thrown when the PSC instance is not initialized.</exception>
        /// <exception cref="GroupNotFoundException">Thrown when the group is not found.</exception>
        /// <exception cref="PlayerNotFoundException">Thrown when the player is not found.</exception>
        public void RemovePlayer(string PlayerID, string GroupName)
        {
            if (!Initialized)
                throw new NotInitializedException();

            if (!_groups.ContainsKey(GroupName))
                throw new GroupNotFoundException();

            if (!_groups[GroupName].Players.Contains(PlayerID))
                throw new PlayerNotFoundException();

            _groups[GroupName].Players.Remove(PlayerID);
        }
    }
}
