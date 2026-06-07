import React, { useState, useEffect } from 'react';
import Button from '../../../components/common/Button';
import { searchSites } from '../../../api/sitesApi';
import { ARTIFACT_TYPES, formatEnum } from '../../../utils/constants';

const DEFAULT_FORM = {
  siteId: '',
  name: '',
  description: '',
  type: 'other',
  displayOrder: '',
};

function Section({ title, children }) {
  return (
    <div style={{ marginBottom: 24 }}>
      <h3 className="section-title">{title}</h3>
      {children}
    </div>
  );
}

export default function ArtifactForm({ initial = null, onSubmit, loading, onCancel }) {
  const isEdit = !!initial;
  const [pickerOpen, setPickerOpen] = useState(false);

  const [form, setForm] = useState(() => {
    if (!initial) return DEFAULT_FORM;
    return {
      ...DEFAULT_FORM,
      ...initial,
      displayOrder: initial.displayOrder || '',
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
        displayOrder: initial.displayOrder || '',
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

  const handleSubmit = (e) => {
    e.preventDefault();

    if (isEdit) {
      const payload = {
        type: form.type,
        displayOrder: form.displayOrder
      };
      onSubmit(payload);
    } else {
      const payload = {
        siteId: form.siteId,
        name: form.name || undefined,
        description: form.description || undefined,
        type: form.type,
        displayOrder: form.displayOrder
      };
      onSubmit(payload);
    }
  };

  return (
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
            <label>Artifact Type *</label>
            <select value={form.type} onChange={(e) => set('type', e.target.value)} required>
              {ARTIFACT_TYPES.map((t) => (
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
            placeholder="Brief description of the artifact…"
            rows={3}
          />
        </div>

        <div className="form-group">
          <label>Display Order</label>
          <input
            type="number"
            value={form.displayOrder || ''}
            onChange={(e) => set('displayOrder', parseInt(e.target.value) || 0)}
            placeholder="Enter display order…"
          />
        </div>
      </Section>

      {/* Actions */}
      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 8 }}>
        <Button type="button" variant="secondary" onClick={onCancel} disabled={loading}>
          Cancel
        </Button>
        <Button type="submit" loading={loading}>
          {isEdit ? 'Save Changes' : 'Create Artifact'}
        </Button>
      </div>
    </form>
  );
}
