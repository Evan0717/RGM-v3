using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exiled.API.Features;
using MEC;
using RGM.UserSettings;
using static RGM.Variables.Variable;

namespace RGM.API.Features
{
    public static class FileManager
    {
        public static string FolderPath => Path.Combine(Paths.Configs, "RGM");

        public static void CreateFolder()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
        }

        public static void WriteFile(string fileName, string content)
        {
            string tempFile = Path.Combine(FolderPath, fileName + ".tmp");

            File.WriteAllText(tempFile, content, Encoding.UTF8);

            string backupFile = Path.Combine(FolderPath, fileName + ".bak");
            if (File.Exists(Path.Combine(FolderPath, fileName)))
            {
                File.Copy(Path.Combine(FolderPath, fileName), backupFile, true);
            }

            File.Replace(tempFile, Path.Combine(FolderPath, fileName), backupFile, true);
        }

        public static string ReadFile(string fileName)
        {
            if (!File.Exists(Path.Combine(FolderPath, fileName)))
                File.WriteAllText(Path.Combine(FolderPath, fileName), "");

            return File.ReadAllText(Path.Combine(FolderPath, fileName));
        }
    }

    public static class UsersManager
    {
        /*
        Exp - 0
        랜덤코인 - 1
        Cash - 2
        보유한 킬 이펙트 - 3
        장착한 킬 이펙트 - 4
        커스텀 닉네임 - 5
        커스텀 인포 - 6
        보유한 커스터마이징 - 7
        보유한 페인트 - 8
        장착한 페인트 - 9
        보유한 칭호 - 10
        장착한 칭호 - 11
        닉네임 - 12
        연동된 디스코드 ID - 13
        연동 코드 - 14
        킬이펙트 랜덤 여부 - 15
        페인트 랜덤 여부 - 16
        칭호 랜덤 여부 - 17
        보유한 아이템 - 18
        보유한 스폰이펙트 - 19
        장착한 스폰이펙트 - 20
        스폰이펙트 랜덤 여부 - 21
        경고 - 22
        방해 금지 활성화 여부 - 23
        보유한 아이콘 - 24
        장착한 아이콘 - 25
        아이콘 랜덤 여부 - 26
        출석 일수 - 27
        최대 연속 출석 일수 - 28
        출석 여부 (오늘) - 29
        */

        public static string UsersFileName = Path.Combine(Paths.Configs, "RGM/Users.txt");
        public static Dictionary<string, List<string>> UsersCache = new Dictionary<string, List<string>>();

        public static string CheckUser(string userId, int num)
        {
            if (UsersCache.ContainsKey(userId) && num >= 0 && num < UsersCache[userId].Count)
                return UsersCache[userId][num];

            return null;
        }

        public static bool AddUser(string userId, List<string> UserInfo) 
        {
            UsersCache[userId] = UserInfo;

            return true;
        }

        public static void SaveUsers()
        {
            if (IsUsersFileLoaded)
            {
                Timing.RunCoroutine(RefreshDiscordId());

                var text = string.Join("\n", UsersCache.Select(x => $"{x.Key};{string.Join(";", x.Value)}"));

                FileManager.WriteFile(UsersFileName, text);
            }
        }

        public static void LoadUsers()
        {
            Timing.RunCoroutine(RefreshDiscordId());

            var text = FileManager.ReadFile(UsersFileName);

            if (string.IsNullOrWhiteSpace(text))
                return;

            UsersCache.Clear();

            foreach (var line in text.Split('\n'))
            {
                var parts = line.Split(';');

                if (parts.Length != parts.Count())
                    continue;

                UsersCache.Add(parts[0], parts.Skip(1).ToList());
            }

            IsUsersFileLoaded = true;
        }

        public static IEnumerator<float> RefreshDiscordId()
        {
            var validUsers = UsersManager.UsersCache
            .Where(x => x.Value.Count > 13 && x.Value[13] != "0")
            .GroupBy(userData => userData.Value[13])
            .ToDictionary(group => group.Key, group => group.First().Key);

            DiscordIdToUserId = validUsers;

            yield break;
        }
    }
}
