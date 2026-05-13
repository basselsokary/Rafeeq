import React, { useState, useEffect } from 'react';
import Button from '../../../components/common/Button';
import { getCities } from '../../../api/citiesApi';
import { SITE_TYPES, LANGUAGE_CODES, formatEnum } from '../../../utils/constants';

const DEFAULT_FORM = {
  cityId: '',
  name: '',
  description: '',
  type: 'historical',
  address: '',
  estimatedDurationMinutes: '',
  location: { latitude: '', longitude: '' },
  ticket: {
    isFree: false,
    egyptianTicketPrice: '',
    foreignerTicketPrice: '',
    foreignerCurrency: 'USD',
    notes: '',
  },
  contactInfo: { phone: '', websiteUrl: '' },
  localizedContents: [],
};

function Section({ title, children }) {
  return (
    <div style={{ marginBottom: 24 }}>
      <h3 className="section-title">{title}</h3>
      {children}
    </div>
  );
}

export default function SiteForm({ initial = null, onSubmit, loading, onCancel }) {
  const [form, setForm] = useState(() => initial
    ? { ...DEFAULT_FORM, ...initial }
    : DEFAULT_FORM
  );
  const [cities, setCities] = useState([]);
  const [citiesLoading, setCitiesLoading] = useState(false);

  const set = (path, value) => {
    setForm((prev) => {
      const clone = JSON.parse(JSON.stringify(prev));
      const keys = path.split('.');
      let obj = clone;
      for (let i = 0; i < keys.length - 1; i++) obj = obj[keys[i]];
      obj[keys[keys.length - 1]] = value;
      return clone;
    });
  };

  useEffect(() => {
    if (initial) setForm({ ...DEFAULT_FORM, ...initial });
  }, [initial]);

  useEffect(() => {
    let active = true;
    const loadCities = async () => {
      setCitiesLoading(true);
      try {
        const res = await getCities();
        const d = res.data;
        const items = Array.isArray(d)
          ? d
          : Array.isArray(d?.value)
            ? d.value
            : Array.isArray(d?.items)
              ? d.items
              : Array.isArray(d?.data)
                ? d.data
                : [];
        if (!active) return;
        setCities(items);
        if (!initial && !form.cityId && items.length) {
          set('cityId', items[0].id ?? items[0].cityId ?? '');
        }
      } finally {
        if (active) setCitiesLoading(false);
      }
    };
    loadCities();
    return () => { active = false; };
  }, [initial]);

  const handleSubmit = (e) => {
    e.preventDefault();
    const payload = {
      ...form,
      estimatedDurationMinutes: Number(form.estimatedDurationMinutes) || undefined,
      location: {
        latitude: parseFloat(form.location.latitude),
        longitude: parseFloat(form.location.longitude),
      },
      ticket: form.ticket.isFree
        ? { isFree: true, notes: form.ticket.notes }
        : {
            isFree: false,
            egyptianTicketPrice: parseFloat(form.ticket.egyptianTicketPrice) || null,
            foreignerTicketPrice: parseFloat(form.ticket.foreignerTicketPrice) || null,
            foreignerCurrency: form.ticket.foreignerCurrency || null,
            notes: form.ticket.notes || null,
          },
      contactInfo: {
        phone: form.contactInfo.phone || null,
        websiteUrl: form.contactInfo.websiteUrl || null,
      },
    };
    if (!payload.name) delete payload.name;
    if (!payload.description) delete payload.description;
    onSubmit(payload);
  };

  const isEdit = !!initial;

  return (
    <form onSubmit={handleSubmit}>
      {/* Basic Info */}
      <Section title="Basic Information">
        {!isEdit && (
          <div className="form-group">
            <label>City *</label>
            <select
              required
              value={form.cityId}
              onChange={(e) => set('cityId', e.target.value)}
              disabled={citiesLoading || cities.length === 0}
            >
              <option value="" disabled>
                {citiesLoading ? 'Loading cities...' : 'Select city'}
              </option>
              {cities.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name || c.id}
                </option>
              ))}
            </select>
          </div>
        )}

        <div className="form-row">
          <div className="form-group">
            <label>Name (default language)</label>
            <input
              value={form.name || ''}
              onChange={(e) => set('name', e.target.value)}
              placeholder="e.g. The Great Pyramid of Giza"
            />
          </div>
          <div className="form-group">
            <label>Site Type *</label>
            <select value={form.type} onChange={(e) => set('type', e.target.value)} required>
              {SITE_TYPES.map((t) => (
                <option key={t} value={t}>{formatEnum(t)}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="form-group">
          <label>Description</label>
          <textarea
            value={form.description || ''}
            onChange={(e) => set('description', e.target.value)}
            placeholder="Brief description of the site…"
            rows={3}
          />
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Address</label>
            <input
              value={form.address || ''}
              onChange={(e) => set('address', e.target.value)}
              placeholder="Street / district"
            />
          </div>
          <div className="form-group">
            <label>Estimated Duration (minutes)</label>
            <input
              type="number" min="0"
              value={form.estimatedDurationMinutes}
              onChange={(e) => set('estimatedDurationMinutes', e.target.value)}
              placeholder="e.g. 120"
            />
          </div>
        </div>
      </Section>

      {/* Location */}
      <Section title="Location Coordinates">
        <div className="form-row">
          <div className="form-group">
            <label>Latitude *</label>
            <input
              type="number" step="any" required
              value={form.location.latitude}
              onChange={(e) => set('location.latitude', e.target.value)}
              placeholder="e.g. 29.9792"
            />
          </div>
          <div className="form-group">
            <label>Longitude *</label>
            <input
              type="number" step="any" required
              value={form.location.longitude}
              onChange={(e) => set('location.longitude', e.target.value)}
              placeholder="e.g. 31.1342"
            />
          </div>
        </div>
      </Section>

      {/* Ticket */}
      <Section title="Entry Ticket">
        <label className="checkbox-group">
          <input
            type="checkbox"
            checked={form.ticket.isFree}
            onChange={(e) => set('ticket.isFree', e.target.checked)}
          />
          <span>Free entry (no ticket required)</span>
        </label>

        {!form.ticket.isFree && (
          <div className="form-row">
            <div className="form-group">
              <label>Egyptian Price (EGP)</label>
              <input
                type="number" step="0.01" min="0"
                value={form.ticket.egyptianTicketPrice}
                onChange={(e) => set('ticket.egyptianTicketPrice', e.target.value)}
                placeholder="0.00"
              />
            </div>
            <div className="form-group">
              <label>Foreigner Price</label>
              <input
                type="number" step="0.01" min="0"
                value={form.ticket.foreignerTicketPrice}
                onChange={(e) => set('ticket.foreignerTicketPrice', e.target.value)}
                placeholder="0.00"
              />
            </div>
            <div className="form-group">
              <label>Foreigner Currency</label>
              <input
                value={form.ticket.foreignerCurrency}
                onChange={(e) => set('ticket.foreignerCurrency', e.target.value)}
                placeholder="USD"
              />
            </div>
          </div>
        )}

        <div className="form-group">
          <label>Ticket Notes</label>
          <input
            value={form.ticket.notes || ''}
            onChange={(e) => set('ticket.notes', e.target.value)}
            placeholder="Student discounts, group rates…"
          />
        </div>
      </Section>

      {/* Contact */}
      <Section title="Contact Information">
        <div className="form-row">
          <div className="form-group">
            <label>Phone</label>
            <input
              value={form.contactInfo.phone || ''}
              onChange={(e) => set('contactInfo.phone', e.target.value)}
              placeholder="+20 2 XXXX XXXX"
            />
          </div>
          <div className="form-group">
            <label>Website URL</label>
            <input
              type="url"
              value={form.contactInfo.websiteUrl || ''}
              onChange={(e) => set('contactInfo.websiteUrl', e.target.value)}
              placeholder="https://..."
            />
          </div>
        </div>
      </Section>

      {/* Actions */}
      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 8 }}>
        <Button type="button" variant="secondary" onClick={onCancel} disabled={loading}>
          Cancel
        </Button>
        <Button type="submit" loading={loading}>
          {isEdit ? 'Save Changes' : 'Create Site'}
        </Button>
      </div>
    </form>
  );
}
