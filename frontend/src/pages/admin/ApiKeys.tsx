import { useState } from 'react';
import { useQuery, useMutation } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { apiFetch } from '../../services/api';

interface ConfigEntry {
  key: string;
  description?: string;
  isSet: boolean;
}

export default function ApiKeysPage() {
  const { t } = useTranslation();
  const [editKey, setEditKey]     = useState('');
  const [editValue, setEditValue] = useState('');
  const [saved, setSaved]         = useState(false);

  const { data: config = [], isLoading } = useQuery({
    queryKey: ['admin-config'],
    queryFn: () => apiFetch<ConfigEntry[]>('/admin/config'),
  });

  const updateMut = useMutation({
    mutationFn: ({ key, value }: { key: string; value: string }) =>
      apiFetch<void>('/admin/config', {
        method: 'POST',
        body: JSON.stringify({ key, value }),
      }),
    onSuccess: () => { setSaved(true); setEditValue(''); setEditKey(''); },
  });

  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-800 mb-6">
        {t('pages.admin.apiKeys')}
      </h1>

      {/* Config status table */}
      <section className="bg-white rounded-xl border border-gray-200 mb-8 overflow-hidden">
        <div className="px-5 py-3 border-b border-gray-100 bg-gray-50">
          <h2 className="text-sm font-semibold text-gray-600">{t('admin.apiKeys.configStatus')}</h2>
        </div>
        {isLoading ? (
          <div className="p-5 text-gray-400 text-sm animate-pulse">{t('common.loading')}</div>
        ) : (
          <table className="w-full text-sm">
            <thead className="border-b border-gray-100">
              <tr>
                <th className="text-left px-5 py-3 text-gray-500 font-medium">Key</th>
                <th className="text-left px-5 py-3 text-gray-500 font-medium">{t('admin.apiKeys.keyValue')}</th>
                <th className="text-left px-5 py-3 text-gray-500 font-medium">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {config.map((entry) => (
                <tr key={entry.key} className="hover:bg-gray-50">
                  <td className="px-5 py-3 font-mono text-gray-700">{entry.key}</td>
                  <td className="px-5 py-3 font-mono text-gray-400">
                    {entry.isSet ? t('admin.apiKeys.masked') : '—'}
                  </td>
                  <td className="px-5 py-3">
                    {entry.isSet ? (
                      <span className="text-green-600 text-xs font-medium">✓ Set</span>
                    ) : (
                      <span className="text-red-500 text-xs font-medium">✗ Missing</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      {/* Update config form */}
      <section className="bg-white rounded-xl border border-gray-200 p-6 max-w-md">
        <h2 className="text-sm font-semibold text-gray-700 mb-4">{t('admin.apiKeys.updateConfig')}</h2>
        {saved && (
          <p className="text-green-600 text-sm mb-3">{t('common.save')} ✓</p>
        )}
        <div className="space-y-3">
          <input
            type="text"
            placeholder="Key"
            value={editKey}
            onChange={(e) => { setEditKey(e.target.value); setSaved(false); }}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm font-mono"
          />
          <input
            type="password"
            placeholder="Value"
            value={editValue}
            onChange={(e) => { setEditValue(e.target.value); setSaved(false); }}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm font-mono"
          />
          <button
            onClick={() => updateMut.mutate({ key: editKey, value: editValue })}
            disabled={updateMut.isPending || !editKey || !editValue}
            className="bg-yellow-500 hover:bg-yellow-600 disabled:opacity-50 text-white px-5 py-2 rounded-lg text-sm font-medium"
          >
            {t('common.save')}
          </button>
        </div>
      </section>
    </div>
  );
}
