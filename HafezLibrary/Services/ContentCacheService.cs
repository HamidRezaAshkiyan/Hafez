using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HafezLibrary.Controllers;
using HafezLibrary.Models;

namespace HafezLibrary.Services
{
    public interface IContentCacheService
    {
        Task WarmUpAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuranModel>> GetQuranAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SureListModel>> GetSureListAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MafatihModel>> GetMafatihAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MafatihListModel>> GetMafatihListAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PersonalDuaModel>> GetPersonalDuasAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PersonalDuaListModel>> GetPersonalDuaListAsync(CancellationToken cancellationToken = default);
        Task RefreshPersonalDuasAsync(CancellationToken cancellationToken = default);

        IEnumerable<QuranModel> SearchQuran(string query, int maxResults = 50);
        IEnumerable<MafatihListModel> SearchMafatih(string query, int maxResults = 50);
        IEnumerable<PersonalDuaListModel> SearchPersonalDuas(string query, int maxResults = 50);
    }

    /// <summary>
    /// Thread-safe asynchronous content cache replacing blocking static DbCache constructors.
    /// Includes Arabic/Persian diacritic-tolerant search.
    /// </summary>
    public sealed class ContentCacheService : IContentCacheService
    {
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private bool _isLoaded;

        private List<QuranModel> _quran = new();
        private List<SureListModel> _sureList = new();
        private List<MafatihModel> _mafatih = new();
        private List<MafatihListModel> _mafatihList = new();
        private List<PersonalDuaModel> _personalDuas = new();
        private List<PersonalDuaListModel> _personalDuaList = new();

        public async Task WarmUpAsync(CancellationToken cancellationToken = default)
        {
            if (_isLoaded)
            {
                return;
            }

            await _initLock.WaitAsync(cancellationToken);
            try
            {
                if (_isLoaded)
                {
                    return;
                }

                await Task.Run(() =>
                {
                    _quran = QuranController.GetAllQuran().ToList();
                    _sureList = SureListController.GetAllSureList();
                    _mafatih = MafatihController.GetAllMafatih();
                    _mafatihList = MafatihListController.GetAllDuas().ToList();
                    _personalDuas = PersonalDuaController.GetAllPersonalDuas();
                    _personalDuaList = PersonalDuaListController.GetAllPersonalDuaList().ToList();
                }, cancellationToken);

                _isLoaded = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<IReadOnlyList<QuranModel>> GetQuranAsync(CancellationToken cancellationToken = default)
        {
            await WarmUpAsync(cancellationToken);
            return _quran;
        }

        public async Task<IReadOnlyList<SureListModel>> GetSureListAsync(CancellationToken cancellationToken = default)
        {
            await WarmUpAsync(cancellationToken);
            return _sureList;
        }

        public async Task<IReadOnlyList<MafatihModel>> GetMafatihAsync(CancellationToken cancellationToken = default)
        {
            await WarmUpAsync(cancellationToken);
            return _mafatih;
        }

        public async Task<IReadOnlyList<MafatihListModel>> GetMafatihListAsync(CancellationToken cancellationToken = default)
        {
            await WarmUpAsync(cancellationToken);
            return _mafatihList;
        }

        public async Task<IReadOnlyList<PersonalDuaModel>> GetPersonalDuasAsync(CancellationToken cancellationToken = default)
        {
            await WarmUpAsync(cancellationToken);
            return _personalDuas;
        }

        public async Task<IReadOnlyList<PersonalDuaListModel>> GetPersonalDuaListAsync(CancellationToken cancellationToken = default)
        {
            await WarmUpAsync(cancellationToken);
            return _personalDuaList;
        }

        public async Task RefreshPersonalDuasAsync(CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                _personalDuas = PersonalDuaController.GetAllPersonalDuas();
                _personalDuaList = PersonalDuaListController.GetAllPersonalDuaList().ToList();
            }, cancellationToken);
        }

        public IEnumerable<QuranModel> SearchQuran(string query, int maxResults = 50)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<QuranModel>();
            }

            string normalized = NormalizeArabicPersian(query);
            return _quran
                .Where(q =>
                    NormalizeArabicPersian(q.AyahText).Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                    NormalizeArabicPersian(q.AyahPersianTranslation).Contains(normalized, StringComparison.OrdinalIgnoreCase))
                .Take(maxResults);
        }

        public IEnumerable<MafatihListModel> SearchMafatih(string query, int maxResults = 50)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return _mafatihList.Take(maxResults);
            }

            string normalized = NormalizeArabicPersian(query);
            return _mafatihList
                .Where(m => NormalizeArabicPersian(m.DuaName).Contains(normalized, StringComparison.OrdinalIgnoreCase))
                .Take(maxResults);
        }

        public IEnumerable<PersonalDuaListModel> SearchPersonalDuas(string query, int maxResults = 50)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return _personalDuaList.Take(maxResults);
            }

            string normalized = NormalizeArabicPersian(query);
            return _personalDuaList
                .Where(d => NormalizeArabicPersian(d.DuaName).Contains(normalized, StringComparison.OrdinalIgnoreCase))
                .Take(maxResults);
        }

        public static string NormalizeArabicPersian(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            Span<char> buffer = stackalloc char[input.Length];
            int idx = 0;

            foreach (char c in input)
            {
                // Strip Arabic harakat / diacritics (U+064B..U+065F, U+0670) and tatweel (U+0640)
                if ((c >= '\u064B' && c <= '\u065F') || c == '\u0670' || c == '\u0640')
                {
                    continue;
                }

                char mapped = c switch
                {
                    'ي' or 'ى' => 'ی',
                    'ك' => 'ک',
                    'أ' or 'إ' or 'آ' => 'ا',
                    'ة' => 'ه',
                    _ => c
                };

                buffer[idx++] = mapped;
            }

            return new string(buffer[..idx]);
        }
    }
}
