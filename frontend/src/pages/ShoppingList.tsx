import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';

interface ShoppingItem {
  id: string;
  name: string;
  acquired: boolean;
}

function loadList(): ShoppingItem[] {
  try {
    const raw = localStorage.getItem('shoppingList');
    if (!raw) return [];
    const parsed: unknown = JSON.parse(raw);
    // Support both string[] (from recipe detail) and ShoppingItem[]
    if (Array.isArray(parsed)) {
      return parsed.map((entry, i) => {
        if (typeof entry === 'string')
          return { id: `${i}`, name: entry, acquired: false };
        const obj = entry as ShoppingItem;
        return { id: obj.id ?? String(i), name: obj.name, acquired: obj.acquired ?? false };
      });
    }
    return [];
  } catch {
    return [];
  }
}

function saveList(items: ShoppingItem[]) {
  localStorage.setItem('shoppingList', JSON.stringify(items));
}

export default function ShoppingListPage() {
  const { t } = useTranslation();
  const [items, setItems]   = useState<ShoppingItem[]>(loadList);
  const [newName, setNewName] = useState('');

  // Sync to localStorage on every change
  useEffect(() => { saveList(items); }, [items]);

  const addItem = (e: React.FormEvent) => {
    e.preventDefault();
    const name = newName.trim();
    if (!name) return;
    setItems((prev) => [...prev, { id: Date.now().toString(), name, acquired: false }]);
    setNewName('');
  };

  const toggle = (id: string) =>
    setItems((prev) =>
      prev.map((it) => (it.id === id ? { ...it, acquired: !it.acquired } : it)),
    );

  const clearCompleted = () => setItems((prev) => prev.filter((it) => !it.acquired));

  const completedCount = items.filter((it) => it.acquired).length;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold text-gray-800">{t('pages.shoppingList')}</h1>
        {completedCount > 0 && (
          <button
            onClick={clearCompleted}
            className="text-sm text-red-500 hover:underline"
          >
            {t('shoppingList.clearCompleted')} ({completedCount})
          </button>
        )}
      </div>

      {/* Add item form */}
      <form onSubmit={addItem} className="flex gap-2 mb-6">
        <input
          type="text"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          placeholder={t('shoppingList.itemName')}
          aria-label={t('shoppingList.addItem')}
          className="flex-1 border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
        />
        <button
          type="submit"
          className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg text-sm font-medium"
        >
          {t('shoppingList.addItem')}
        </button>
      </form>

      {/* List */}
      {items.length === 0 ? (
        <div className="text-center py-16 text-gray-400">
          <p className="text-3xl mb-2">🛒</p>
          <p>{t('shoppingList.noItems')}</p>
        </div>
      ) : (
        <ul className="bg-white rounded-xl border border-gray-200 divide-y divide-gray-100">
          {items.map((item) => (
            <li key={item.id} className="flex items-center gap-3 px-4 py-3">
              <input
                type="checkbox"
                id={`item-${item.id}`}
                checked={item.acquired}
                onChange={() => toggle(item.id)}
                aria-label={`${t('shoppingList.markAcquired')}: ${item.name}`}
                className="w-4 h-4 rounded text-green-600 focus:ring-green-500"
              />
              <label
                htmlFor={`item-${item.id}`}
                className={`flex-1 text-sm cursor-pointer ${
                  item.acquired ? 'line-through text-gray-400' : 'text-gray-800'
                }`}
              >
                {item.name}
              </label>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
