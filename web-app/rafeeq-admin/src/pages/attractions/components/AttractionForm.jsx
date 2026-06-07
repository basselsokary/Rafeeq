import React, { useState, useEffect } from 'react';
import Button from '../../../components/common/Button';
import { searchSites } from '../../../api/sitesApi';
import { ATTRACTION_TYPES, HISTORICAL_PERIODS, formatEnum } from '../../../utils/constants';
import LocationPickerModal from '../../../components/map/LocationPickerModal';

const DEFAULT_FORM = {
  siteId: '',
  name: '',
  description: '',
  locationDescription: '',
  type: 'other',
  isFeatured: false,
  location: { latitude: '', longitude: '' },
  historicalPeriod: [],
};

function Section({ title, children }) {
  return (
    <div style={{ marginBottom: 24 }}>
      <h3 className="section-title">{title}</h3>
      {children}
    </div>
  );
}

export default function AttractionForm({ initial = null, onSubmit, loading, onCancel }) {
  const isEdit = !!initial;
  const [pickerOpen, setPickerOpen] = useState(false);

  const [form, setForm] = useState(() => {
    if (!initial) return DEFAULT_FORM;
    return {
      ...DEFAULT_FORM,
      ...initial,
      historicalPeriod: initial.historicalPeriods || initial.historicalPeriod || [],
      location: initial.location || initial.exactLocation || { latitude: '', longitude: '' },
    };
  });
  const [siteQuery, setSiteQuery] = useState('');
  const [siteResults, setSiteResults] = useState([]);
  const [siteSearching, setSiteSearching] = useState(false);
  const [siteTouched, setSiteTouched] = useState(false);

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
    if (initial) {
      setForm({
        ...DEFAULT_FORM,
        ...initial,
        historicalPeriod: initial.historicalPeriods || initial.historicalPeriod || [],
        location: initial.location || initial.exactLocation || { latitude: '', longitude: '' },
      });
    }
  }, [initial]);

  useEffect(() => {
    if (isEdit) return;

    const q = siteQuery.trim();
    if (!q) {
      setSiteResults([]);
      setSiteSearching(false);
      return;
    }

    let cancelled = false;
    const handle = setTimeout(async () => {
      setSiteSearching(true);
      try {
        const res = await searchSites({ q });
        const d = res?.data;
        const items = Array.isArray(d)
          ? d
          : (d?.data ?? d?.value ?? d?.items ?? d?.results ?? []);

        const list = Array.isArray(items) ? items : [];
        if (!cancelled) setSiteResults(list.slice(0, 10));
      } catch {
        if (!cancelled) setSiteResults([]);
      } finally {
        if (!cancelled) setSiteSearching(false);
      }
    }, 700);

    return () => {
      cancelled = true;
      clearTimeout(handle);
    };
  }, [isEdit, siteQuery]);

  const togglePeriod = (period) => {
    setForm(prev => {
      const list = prev.historicalPeriod || [];
      return {
        ...prev,
        historicalPeriod: list.includes(period)
          ? list.filter(p => p !== period)
          : [...list, period],
      };
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (isEdit) {
      const payload = {
        type: form.type,
        historicalPeriod: form.historicalPeriod,
        exactLocation: {
          latitude: parseFloat(form.location.latitude),
          longitude: parseFloat(form.location.longitude),
        },
      };
      onSubmit(payload);
    } else {
      const payload = {
        siteId: form.siteId,
        name: form.name || undefined,
        description: form.description || undefined,
        locationDescription: form.locationDescription || undefined,
        type: form.type,
        isFeatured: form.isFeatured,
        location: {
          latitude: parseFloat(form.location.latitude),
          longitude: parseFloat(form.location.longitude),
        },
        historicalPeriod: form.historicalPeriod.length ? form.historicalPeriod : undefined,
      };
      onSubmit(payload);
    }
  };

  const handleLocationConfirm = ({ latitude, longitude }) => {
    set('location.latitude', latitude.toFixed(9));
    set('location.longitude', longitude.toFixed(9));
  };

  const hasLocation =
    form.location.latitude !== '' && form.location.latitude !== null &&
    form.location.longitude !== '' && form.location.longitude !== null &&
    !isNaN(parseFloat(form.location.latitude)) &&
    !isNaN(parseFloat(form.location.longitude));

  return (
    <>
      <form onSubmit={handleSubmit}>
        {/* Basic Info */}
        <Section title="Basic Information">
          {!isEdit && (
            <div className="form-group">
              <label>Site *</label>

              <div style={{ position: 'relative' }}>
                <input
                  value={siteQuery}
                  onChange={(e) => {
                    setSiteQuery(e.target.value);
                    setSiteTouched(true);
                    if (form.siteId) set('siteId', '');
                  }}
                  onBlur={() => setTimeout(() => setSiteTouched(true), 0)}
                  placeholder="Search site name (English)…"
                  required={!form.siteId}
                />

                {(siteQuery.trim() || siteTouched) && (
                  <div style={{
                    position: 'absolute',
                    top: 'calc(100% + 6px)',
                    left: 0,
                    right: 0,
                    zIndex: 10,
                    background: 'var(--surface-container-lowest)',
                    border: '1px solid rgba(212,196,183,.35)',
                    borderRadius: 12,
                    overflow: 'hidden',
                    boxShadow: '0 10px 25px rgba(29,27,23,.08)',
                  }}>
                    {siteSearching ? (
                      <div style={{ padding: '10px 12px', fontSize: 12, color: 'var(--text-2)' }}>
                        Searching…
                      </div>
                    ) : siteResults.length ? (
                      siteResults.map((s) => (
                        <button
                          key={s.id}
                          type="button"
                          onMouseDown={(e) => e.preventDefault()}
                          onClick={() => {
                            set('siteId', s.id ?? '');
                            setSiteQuery(s.name || s.title || s.id || '');
                            setSiteResults([]);
                          }}
                          style={{
                            width: '100%',
                            textAlign: 'left',
                            padding: '10px 12px',
                            border: 'none',
                            background: 'transparent',
                            cursor: 'pointer',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'space-between',
                            gap: 12,
                          }}
                        >
                          <span style={{ fontSize: 13, color: 'var(--text)' }}>
                            {s.name || s.title || s.id}
                          </span>
                          <span style={{ fontSize: 11, color: 'var(--outline)' }}>
                            {s.id}
                          </span>
                        </button>
                      ))
                    ) : (
                      <div style={{ padding: '10px 12px', fontSize: 12, color: 'var(--text-2)' }}>
                        No sites found.
                      </div>
                    )}
                  </div>
                )}
              </div>

              {form.siteId ? (
                <div style={{ marginTop: 8, display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 10 }}>
                  <div style={{ fontSize: 11, color: 'var(--outline)' }}>
                    Selected: <span style={{ color: 'var(--text-2)', fontWeight: 600 }}>{form.siteId}</span>
                  </div>
                  <button
                    type="button"
                    onClick={() => {
                      set('siteId', '');
                      setSiteQuery('');
                      setSiteResults([]);
                    }}
                    style={{
                      border: 'none',
                      background: 'transparent',
                      cursor: 'pointer',
                      color: 'var(--text-2)',
                      fontSize: 12,
                      fontWeight: 600,
                      padding: 0,
                    }}
                  >
                    Clear
                  </button>
                </div>
              ) : null}
            </div>
          )}

          <div className="form-row">
            <div className="form-group">
              <label>{isEdit ? 'Name (read-only)' : 'Name *'}</label>
              <input
                value={form.name || ''}
                onChange={(e) => set('name', e.target.value)}
                placeholder="e.g. The Great Pyramid of Khufu"
                disabled={isEdit}
                required={!isEdit}
              />
            </div>
            <div className="form-group">
              <label>Attraction Type *</label>
              <select value={form.type} onChange={(e) => set('type', e.target.value)} required>
                {ATTRACTION_TYPES.map((t) => (
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
              placeholder="Brief description of the attraction…"
              rows={3}
            />
          </div>

          {!isEdit && (
            <div className="form-group">
              <label>Location Description</label>
              <input
                value={form.locationDescription || ''}
                onChange={(e) => set('locationDescription', e.target.value)}
                placeholder="e.g. North side of the complex"
              />
            </div>
          )}

          {!isEdit && (
            <label className="checkbox-group" style={{ display: 'flex', alignItems: 'center', gap: 9, cursor: 'pointer', marginBottom: 10 }}>
              <input
                type="checkbox"
                checked={form.isFeatured}
                onChange={(e) => set('isFeatured', e.target.checked)}
                style={{ width: 16, height: 16, accentColor: 'var(--primary)' }}
              />
              <span style={{ fontSize: 13, fontWeight: 500, color: 'var(--text-2)', textTransform: 'none', letterSpacing: 0 }}>Mark as Featured</span>
            </label>
          )}
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

        {/* Historical Periods */}
        <Section title="Historical Periods">
          <div style={{
            display: 'flex', flexWrap: 'wrap', gap: 8, marginBottom: 8,
            maxHeight: 180, overflowY: 'auto',
            padding: '12px 14px', background: 'var(--surface-container-low)',
            borderRadius: 10, border: '1px solid rgba(212,196,183,.3)',
          }}>
            {HISTORICAL_PERIODS.map(p => {
              const isSelected = (form.historicalPeriod || []).includes(p);
              return (
                <button
                  key={p} type="button"
                  onClick={() => togglePeriod(p)}
                  style={{
                    padding: '5px 12px', borderRadius: 20, fontSize: 11, fontWeight: 600,
                    cursor: 'pointer', transition: 'all .15s',
                    border: isSelected ? '1px solid var(--primary)' : '1px solid rgba(212,196,183,.4)',
                    background: isSelected ? 'rgba(124,87,45,.1)' : 'var(--surface-container-lowest)',
                    color: isSelected ? 'var(--primary)' : 'var(--text-2)',
                  }}
                >
                  {formatEnum(p)}
                </button>
              );
            })}
          </div>
          {(form.historicalPeriod || []).length > 0 && (
            <div style={{ fontSize: 11, color: 'var(--outline)' }}>
              {form.historicalPeriod.length} period{form.historicalPeriod.length !== 1 ? 's' : ''} selected
            </div>
          )}
        </Section>

        {/* Actions */}
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 8 }}>
          <Button type="button" variant="secondary" onClick={onCancel} disabled={loading}>
            Cancel
          </Button>
          <Button type="submit" loading={loading}>
            {isEdit ? 'Save Changes' : 'Create Attraction'}
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
