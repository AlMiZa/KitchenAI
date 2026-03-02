import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import {
  getItems,
  createItem,
  updateItem,
  deleteItem,
} from '../services/items';
import type { InventoryItem, CreateItemInput } from '../services/items';

type StorageLocation = 'fridge' | 'freezer' | 'pantry' | 'other';
type ItemUnit = 'g' | 'kg' | 'ml' | 'L' | 'pcs';

interface ItemFormData {
  name: string;
  quantity: string;
  unit: ItemUnit;
  purchaseDate: string;
  expiryDate: string;
  expiryType: 'bestBy' | 'useBy';
  storageLocation: StorageLocation | '';
  brand: string;
  price: string;
  notes: string;
}

const DEFAULT_FORM: ItemFormData = {
  name: '',
  quantity: '',
  unit: 'pcs',
  purchaseDate: '',
  expiryDate: '',
  expiryType: 'bestBy',
  storageLocation: '',
  brand: '',
  price: '',
  notes: '',
};

function daysUntilExpiry(dateStr: string): number {
  const expiry = new Date(dateStr);
  const now = new Date();
  expiry.setHours(0, 0, 0, 0);
  now.setHours(0, 0, 0, 0);
  return Math.ceil((expiry.getTime() - now.getTime()) / 86_400_000);
}

function ExpiryBadge({ expiryDate }: { expiryDate?: string }) {
  if (!expiryDate) return null;
  const days = daysUntilExpiry(expiryDate);
  if (days < 0)
    return (
      <span className="px-2 py-0.5 bg-red-100 text-red-700 rounded text-xs font-medium">
        Expired
      </span>
    );
  if (days <= 3)
    return (
      <span className="px-2 py-0.5 bg-amber-100 text-amber-700 rounded text-xs font-medium">
        {days === 0 ? 'Today' : `${days}d`}
      </span>
    );
  return null;
}

export default function InventoryPage() {
  const { t } = useTranslation();
  const { householdId } = useAuth();
  const queryClient = useQueryClient();

  const [locationFilter, setLocationFilter]         = useState('');
  const [expiringSoonFilter, setExpiringSoonFilter] = useState(false);
  const [showModal, setShowModal]                   = useState(false);
  const [editingItem, setEditingItem]               = useState<InventoryItem | null>(null);
  const [form, setForm]                             = useState<ItemFormData>(DEFAULT_FORM);
  const [formErrors, setFormErrors]                 = useState<Partial<Record<keyof ItemFormData, string>>>({});

  const { data: items = [], isLoading } = useQuery({
    queryKey: ['items', householdId],
    queryFn: () => getItems(householdId!),
    enabled: !!householdId,
  });

  const createMut = useMutation({
    mutationFn: (data: CreateItemInput) => createItem(householdId!, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['items', householdId] }); closeModal(); },
    onError: (err: Error) => setFormErrors({ name: err.message || t('common.error') }),
  });

  const updateMut = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<CreateItemInput> }) =>
      updateItem(householdId!, id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['items', householdId] }); closeModal(); },
    onError: (err: Error) => setFormErrors({ name: err.message || t('common.error') }),
  });

  const deleteMut = useMutation({
    mutationFn: (id: string) => deleteItem(householdId!, id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['items', householdId] }),
  });

  // Sort ascending by expiry date (items without expiry go last)
  const sorted = [...items].sort((a, b) => {
    if (!a.expiryDate && !b.expiryDate) return 0;
    if (!a.expiryDate) return 1;
    if (!b.expiryDate) return -1;
    return new Date(a.expiryDate).getTime() - new Date(b.expiryDate).getTime();
  });

  const displayed = sorted.filter((item) => {
    if (locationFilter && item.storageLocation !== locationFilter) return false;
    if (expiringSoonFilter) {
      if (!item.expiryDate) return false;
      if (daysUntilExpiry(item.expiryDate) > 3) return false;
    }
    return true;
  });

  const openAdd = () => {
    setEditingItem(null);
    setForm(DEFAULT_FORM);
    setFormErrors({});
    setShowModal(true);
  };

  const openEdit = (item: InventoryItem) => {
    setEditingItem(item);
    setForm({
      name:            item.name,
      quantity:        String(item.quantity),
      unit:            item.unit,
      purchaseDate:    item.purchaseDate    ?? '',
      expiryDate:      item.expiryDate      ?? '',
      expiryType:      item.bestByOrUseBy   ?? 'bestBy',
      storageLocation: item.storageLocation ?? '',
      brand:           item.brand           ?? '',
      price:           item.price !== undefined ? String(item.price) : '',
      notes:           item.notes           ?? '',
    });
    setFormErrors({});
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingItem(null);
    setForm(DEFAULT_FORM);
    setFormErrors({});
  };

  const validateForm = (): boolean => {
    const errs: Partial<Record<keyof ItemFormData, string>> = {};
    if (!form.name.trim()) errs.name = t('auth.required');
    if (form.quantity !== '') {
      const n = parseFloat(form.quantity);
      if (isNaN(n) || n < 0) errs.quantity = t('inventory.invalidQty');
    }
    setFormErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!householdId) { setFormErrors({ name: t('auth.pleaseLogin') }); return; }
    if (!validateForm()) return;
    const payload: CreateItemInput = {
      name:            form.name.trim(),
      quantity:        form.quantity ? parseFloat(form.quantity) : 0,
      unit:            form.unit,
      purchaseDate:    form.purchaseDate    || undefined,
      expiryDate:      form.expiryDate      || undefined,
      bestByOrUseBy:   form.expiryType,
      storageLocation: (form.storageLocation as StorageLocation) || undefined,
      brand:           form.brand           || undefined,
      price:           form.price ? parseFloat(form.price) : undefined,
      notes:           form.notes           || undefined,
    };
    if (editingItem) {
      updateMut.mutate({ id: editingItem.id, data: payload });
    } else {
      createMut.mutate(payload);
    }
  };

  const isPending = createMut.isPending || updateMut.isPending;

  return (
    <div>
      {/* Page header */}
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold text-gray-800">{t('pages.inventory')}</h1>
        <button
          onClick={openAdd}
          className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg text-sm font-medium"
        >
          + {t('inventory.addItem')}
        </button>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-center gap-3 mb-5 p-3 bg-white rounded-lg border border-gray-200">
        <select
          value={locationFilter}
          onChange={(e) => setLocationFilter(e.target.value)}
          aria-label={t('inventory.filterByLocation')}
          className="text-sm border border-gray-300 rounded-md px-2 py-1.5 focus:outline-none focus:ring-2 focus:ring-green-500"
        >
          <option value="">{t('inventory.allLocations')}</option>
          <option value="fridge">{t('inventory.locations.fridge')}</option>
          <option value="freezer">{t('inventory.locations.freezer')}</option>
          <option value="pantry">{t('inventory.locations.pantry')}</option>
          <option value="other">{t('inventory.locations.other')}</option>
        </select>

        <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer select-none">
          <input
            type="checkbox"
            checked={expiringSoonFilter}
            onChange={(e) => setExpiringSoonFilter(e.target.checked)}
            className="rounded"
          />
          {t('inventory.expiringSoon')}
        </label>
      </div>

      {/* Table / empty states */}
      {isLoading ? (
        <div className="text-center py-16 text-gray-400">{t('common.loading')}</div>
      ) : displayed.length === 0 ? (
        <div className="text-center py-16 text-gray-400">{t('inventory.noItems')}</div>
      ) : (
        <div className="bg-white rounded-xl border border-gray-200 overflow-x-auto">
          <table className="w-full text-sm" aria-label={t('pages.inventory')}>
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="text-left px-4 py-3 text-gray-500 font-medium">{t('inventory.name')}</th>
                <th className="text-left px-4 py-3 text-gray-500 font-medium">{t('inventory.quantity')}</th>
                <th className="text-left px-4 py-3 text-gray-500 font-medium">{t('inventory.expiryDate')}</th>
                <th className="text-left px-4 py-3 text-gray-500 font-medium">{t('inventory.location')}</th>
                <th className="text-left px-4 py-3 text-gray-500 font-medium">Status</th>
                <th className="text-right px-4 py-3 text-gray-500 font-medium">{t('common.edit')}</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {displayed.map((item) => (
                <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-800">{item.name}</td>
                  <td className="px-4 py-3 text-gray-600">{item.quantity} {item.unit}</td>
                  <td className="px-4 py-3 text-gray-600">
                    {item.expiryDate ? new Date(item.expiryDate).toLocaleDateString() : '—'}
                  </td>
                  <td className="px-4 py-3 text-gray-600 capitalize">{item.storageLocation ?? '—'}</td>
                  <td className="px-4 py-3">
                    <ExpiryBadge expiryDate={item.expiryDate} />
                  </td>
                  <td className="px-4 py-3 text-right space-x-3">
                    <button
                      onClick={() => openEdit(item)}
                      aria-label={`${t('common.edit')} ${item.name}`}
                      className="text-blue-600 hover:underline text-xs"
                    >
                      {t('common.edit')}
                    </button>
                    <button
                      onClick={() => deleteMut.mutate(item.id)}
                      aria-label={`${t('common.delete')} ${item.name}`}
                      className="text-red-500 hover:text-red-700 text-base"
                    >
                      🗑
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Add / Edit Modal */}
      {showModal && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
          role="dialog"
          aria-modal="true"
          aria-labelledby="item-modal-title"
        >
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[90vh] overflow-y-auto">
            {/* Modal header */}
            <div className="flex items-center justify-between p-6 border-b border-gray-100">
              <h2 id="item-modal-title" className="text-lg font-semibold text-gray-800">
                {editingItem ? t('inventory.editItem') : t('inventory.addNewItem')}
              </h2>
              <button onClick={closeModal} aria-label={t('common.close')} className="text-gray-400 hover:text-gray-600 text-xl leading-none">
                ✕
              </button>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              {/* Name */}
              <div>
                <label htmlFor="item-name" className="block text-sm font-medium text-gray-700 mb-1">
                  {t('inventory.name')} <span className="text-red-500">*</span>
                </label>
                <input
                  id="item-name"
                  type="text"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                />
                {formErrors.name && <p className="text-red-500 text-xs mt-1">{formErrors.name}</p>}
              </div>

              {/* Quantity + Unit */}
              <div className="flex gap-3">
                <div className="flex-1">
                  <label htmlFor="item-quantity" className="block text-sm font-medium text-gray-700 mb-1">
                    {t('inventory.quantity')}
                  </label>
                  <input
                    id="item-quantity"
                    type="number"
                    step="any"
                    min="0"
                    value={form.quantity}
                    onChange={(e) => setForm({ ...form, quantity: e.target.value })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                  />
                  {formErrors.quantity && <p className="text-red-500 text-xs mt-1">{formErrors.quantity}</p>}
                </div>
                <div className="w-24">
                  <label htmlFor="item-unit" className="block text-sm font-medium text-gray-700 mb-1">
                    {t('inventory.unit')}
                  </label>
                  <select
                    id="item-unit"
                    value={form.unit}
                    onChange={(e) => setForm({ ...form, unit: e.target.value as ItemUnit })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                  >
                    <option value="g">g</option>
                    <option value="kg">kg</option>
                    <option value="ml">ml</option>
                    <option value="L">L</option>
                    <option value="pcs">pcs</option>
                  </select>
                </div>
              </div>

              {/* Purchase Date */}
              <div>
                <label htmlFor="item-purchase" className="block text-sm font-medium text-gray-700 mb-1">
                  {t('inventory.purchaseDate')}
                </label>
                <input id="item-purchase" type="date" value={form.purchaseDate}
                  onChange={(e) => setForm({ ...form, purchaseDate: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
              </div>

              {/* Expiry Date + Type */}
              <div>
                <label htmlFor="item-expiry" className="block text-sm font-medium text-gray-700 mb-1">
                  {t('inventory.expiryDate')}
                </label>
                <input id="item-expiry" type="date" value={form.expiryDate}
                  onChange={(e) => setForm({ ...form, expiryDate: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
                <div className="flex gap-6 mt-2">
                  {(['bestBy', 'useBy'] as const).map((type) => (
                    <label key={type} className="flex items-center gap-1.5 text-sm text-gray-600 cursor-pointer">
                      <input type="radio" name="expiry-type" value={type}
                        checked={form.expiryType === type}
                        onChange={() => setForm({ ...form, expiryType: type })} />
                      {type === 'bestBy' ? t('inventory.bestBy') : t('inventory.useBy')}
                    </label>
                  ))}
                </div>
              </div>

              {/* Storage Location */}
              <div>
                <label htmlFor="item-location" className="block text-sm font-medium text-gray-700 mb-1">
                  {t('inventory.location')}
                </label>
                <select id="item-location" value={form.storageLocation}
                  onChange={(e) => setForm({ ...form, storageLocation: e.target.value as StorageLocation | '' })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500">
                  <option value="">—</option>
                  <option value="fridge">{t('inventory.locations.fridge')}</option>
                  <option value="freezer">{t('inventory.locations.freezer')}</option>
                  <option value="pantry">{t('inventory.locations.pantry')}</option>
                  <option value="other">{t('inventory.locations.other')}</option>
                </select>
              </div>

              {/* Brand + Price */}
              <div className="flex gap-3">
                <div className="flex-1">
                  <label htmlFor="item-brand" className="block text-sm font-medium text-gray-700 mb-1">{t('inventory.brand')}</label>
                  <input id="item-brand" type="text" value={form.brand}
                    onChange={(e) => setForm({ ...form, brand: e.target.value })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm" />
                </div>
                <div className="w-28">
                  <label htmlFor="item-price" className="block text-sm font-medium text-gray-700 mb-1">{t('inventory.price')}</label>
                  <input id="item-price" type="number" step="0.01" min="0" value={form.price}
                    onChange={(e) => setForm({ ...form, price: e.target.value })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm" />
                </div>
              </div>

              {/* Notes */}
              <div>
                <label htmlFor="item-notes" className="block text-sm font-medium text-gray-700 mb-1">{t('inventory.notes')}</label>
                <textarea id="item-notes" rows={2} value={form.notes}
                  onChange={(e) => setForm({ ...form, notes: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm resize-none" />
              </div>

              {/* Action buttons */}
              <div className="flex gap-3 pt-2">
                <button type="submit" disabled={isPending}
                  className="flex-1 bg-green-600 hover:bg-green-700 disabled:opacity-50 text-white py-2 rounded-lg text-sm font-medium">
                  {isPending ? t('common.loading') : t('common.save')}
                </button>
                <button type="button" onClick={closeModal}
                  className="px-5 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-600 hover:bg-gray-50">
                  {t('common.cancel')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
