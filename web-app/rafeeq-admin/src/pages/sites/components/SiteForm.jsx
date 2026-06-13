import React, { useState, useEffect } from 'react';
import Button from '../../../components/common/Button';
import { getCities } from '../../../api/citiesApi';
import { SITE_TYPES, LANGUAGE_CODES, formatEnum } from '../../../utils/constants';
import LocationPickerModal from '../../../components/map/LocationPickerModal';

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
};

// Maps the Backend GET shape to our Frontend Form State
const normalizeInitial = (data) => {
  if (!data) return DEFAULT_FORM;
  return {
    ...DEFAULT_FORM,
    cityId: data.cityId || '', 
    name: data.name || '',
    description: data.description || '',
    type: data.type || 'historical',
    address: data.address || '',
    estimatedDurationMinutes: data.estimatedDurationMinutes || '',
    location: {
      latitude: data.location?.latitude ?? '',
      longitude: data.location?.longitude ?? '',
    },
    ticket: {
      isFree: data.isFree || false,
      egyptianTicketPrice: data.entryTicket?.egyptianTicketPrice?.amount ?? '',
      // Accommodates the 'foreingerTicketPrice' typo from the backend GET response
      foreignerTicketPrice: data.entryTicket?.foreingerTicketPrice?.amount ?? '',
      foreignerCurrency: data.entryTicket?.foreingerTicketPrice?.currency || 'USD',
      notes: data.entryTicket?.notes || '',
    },
    contactInfo: {
      phone: data.contactPhone || '',
      websiteUrl: data.website || '',
    },
  };
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
  const isEdit = !!initial;
  
  const [form, setForm] = useState(() => normalizeInitial(initial));
  const [cities, setCities] = useState([]);
  const [citiesLoading, setCitiesLoading] = useState(false);
  const [pickerOpen, setPickerOpen] = useState(false);

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
    setForm(normalizeInitial(initial));
  }, [initial]);

  useEffect(() => {
    let active = true;
    const loadCities = async () => {
      if (isEdit) return; // No need to load cities on Edit since we don't update CityId
      setCitiesLoading(true);
      try {
        const res = await getCities();
        const d = res.data;
        const items = Array.isArray(d) ? d
                    : Array.isArray(d?.value) ? d.value
                    : Array.isArray(d?.items) ? d.items
                    : Array.isArray(d?.data) ? d.data
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
  }, [initial, isEdit, form.cityId]);

  /* Called when user confirms a location in the picker */
  const handleLocationConfirm = ({ latitude, longitude }) => {
    set('location.latitude', latitude.toFixed(9));
    set('location.longitude', longitude.toFixed(9));
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    // Standardized Ticket Request Object
    const ticketPayload = {
      isFree: form.ticket.isFree,
      egyptianTicketPrice: form.ticket.isFree ? null : (parseFloat(form.ticket.egyptianTicketPrice) || null),
      foreignerTicketPrice: form.ticket.isFree ? null : (parseFloat(form.ticket.foreignerTicketPrice) || null),
      foreignerCurrency: form.ticket.isFree ? null : (form.ticket.foreignerCurrency || null),
      notes: form.ticket.notes || null,
    };

    // Standardized Contact Info Object
    const contactInfoPayload = {
      phone: form.contactInfo.phone || null,
      websiteUrl: form.contactInfo.websiteUrl || null,
    };

    // Shared Payload properties
    const basePayload = {
      type: form.type,
      location: {
        latitude: parseFloat(form.location.latitude),
        longitude: parseFloat(form.location.longitude),
      },
      estimatedDurationMinutes: parseInt(form.estimatedDurationMinutes, 10) || 0,
      ticket: ticketPayload,
      contactInfo: contactInfoPayload,
    };

    // Split payload based on Endpoint shape requirements
    if (isEdit) {
      onSubmit(basePayload); // UpdateSiteRequest
    } else {
      onSubmit({
        ...basePayload, // CreateSiteRequest
        cityId: form.cityId,
        name: form.name,
        description: form.description,
        address: form.address,
      });
    }
  };

  /* Whether a valid location is already set */
  const hasLocation =
    form.location.latitude !== '' && form.location.latitude !== null &&
    form.location.longitude !== '' && form.location.longitude !== null &&
    !isNaN(parseFloat(form.location.latitude)) &&
    !isNaN(parseFloat(form.location.longitude));

  return (
    <>
      <form onSubmit={handleSubmit}>
        {/* ── Basic Info ── */}
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
                  {citiesLoading ? 'Loading cities…' : 'Select city'}
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
              <label>English Name (default language) {!isEdit && '*'}</label>
              <input
                required={!isEdit}
                disabled={isEdit}
                value={form.name || ''}
                onChange={(e) => set('name', e.target.value)}
                placeholder="e.g. Cairo Citadel"
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
              disabled={isEdit}
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
                disabled={isEdit}
                value={form.address || ''}
                onChange={(e) => set('address', e.target.value)}
                placeholder="Street / district"
              />
            </div>
            <div className="form-group">
              <label>Estimated Duration (minutes)</label>
              <input
                type="number" min="0" required
                value={form.estimatedDurationMinutes}
                onChange={(e) => set('estimatedDurationMinutes', e.target.value)}
                placeholder="e.g. 120"
              />
            </div>
          </div>
        </Section>

        {/* ── Location ── */}
        <Section title="Location Coordinates">
          {/* Map picker trigger */}
          <button
            type="button"
            onClick={() => setPickerOpen(true)}
            style={{
              width: '100%', padding: '11px 16px', marginBottom: 14,
              borderRadius: 10, cursor: 'pointer',
              border: `1.5px dashed ${hasLocation ? 'var(--primary)' : 'var(--outline-variant)'}`,
              background: hasLocation ? 'rgba(124,87,45,0.05)' : 'var(--surface-container-low)',
              display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 9,
              color: hasLocation ? 'var(--primary)' : 'var(--text-2)',
              fontSize: 13, fontWeight: 600,
              fontFamily: 'var(--font-body)',
              transition: 'all .15s',
            }}
          >
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0118 0z"/>
              <circle cx="12" cy="10" r="3"/>
            </svg>
            {hasLocation ? 'Change Location on Map' : 'Select Location on Map'}
            {hasLocation && (
              <span style={{
                marginLeft: 4, fontSize: 11, fontWeight: 500,
                color: 'var(--outline)', fontFamily: 'monospace',
              }}>
                ({parseFloat(form.location.latitude).toFixed(4)}, {parseFloat(form.location.longitude).toFixed(4)})
              </span>
            )}
          </button>

          <div className="form-row">
            <div className="form-group">
              <label>Latitude *</label>
              <input
                type="number" step="any" required
                value={form.location.latitude}
                onChange={(e) => set('location.latitude', e.target.value)}
                placeholder="e.g. 29.979200"
              />
            </div>
            <div className="form-group">
              <label>Longitude *</label>
              <input
                type="number" step="any" required
                value={form.location.longitude}
                onChange={(e) => set('location.longitude', e.target.value)}
                placeholder="e.g. 31.134200"
              />
            </div>
          </div>
        </Section>

        {/* ── Ticket ── */}
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
                  type="number" step="0.01" min="0" required
                  value={form.ticket.egyptianTicketPrice}
                  onChange={(e) => set('ticket.egyptianTicketPrice', e.target.value)}
                  placeholder="0.00"
                />
              </div>
              <div className="form-group">
                <label>Foreigner Price</label>
                <input
                  type="number" step="0.01" min="0" required
                  value={form.ticket.foreignerTicketPrice}
                  onChange={(e) => set('ticket.foreignerTicketPrice', e.target.value)}
                  placeholder="0.00"
                />
              </div>
              <div className="form-group">
                <label>Foreigner Currency</label>
                <input
                  required
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

        {/* ── Contact ── */}
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

        {/* ── Actions ── */}
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 8 }}>
          <Button type="button" variant="secondary" onClick={onCancel} disabled={loading}>
            Cancel
          </Button>
          <Button type="submit" loading={loading}>
            {isEdit ? 'Save Changes' : 'Create Site'}
          </Button>
        </div>
      </form>

      {/* Location picker modal */}
      <LocationPickerModal
        open={pickerOpen}
        onClose={() => setPickerOpen(false)}
        onConfirm={handleLocationConfirm}
        initial={hasLocation ? form.location : null}
      />
    </>
  );
}