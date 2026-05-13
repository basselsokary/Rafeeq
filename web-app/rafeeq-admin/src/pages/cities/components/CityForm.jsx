import React, { useState, useEffect, useRef } from 'react';
import Button from '../../../components/common/Button';

const DEFAULT_FORM = {
  name: '',
  description: '',
  centerLocation: { latitude: '', longitude: '' },
  displayOrder: 1,
};

function Section({ title, children }) {
  return (
    <div style={{ marginBottom: 24 }}>
      <h3 className="section-title">{title}</h3>
      {children}
    </div>
  );
}

export default function CityForm({ initial = null, onSubmit, loading, onCancel }) {
  const isEdit = !!initial;
  const fileRef = useRef(null);

  const [form, setForm] = useState(() => {
    if (!initial) return DEFAULT_FORM;
    return {
      ...DEFAULT_FORM,
      name: initial.name || '',
      description: initial.description || '',
      centerLocation: initial.centerLocation || { latitude: '', longitude: '' },
      displayOrder: initial.displayOrder ?? 1,
    };
  });
  const [imageFile, setImageFile]     = useState(null);
  const [imagePreview, setImagePreview] = useState(initial?.imageUrl || null);
  const [dragOver, setDragOver]       = useState(false);

  const set = (path, value) => {
    setForm(prev => {
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
        name: initial.name || '',
        description: initial.description || '',
        centerLocation: initial.centerLocation || { latitude: '', longitude: '' },
        displayOrder: initial.displayOrder ?? 1,
      });
      setImagePreview(initial.imageUrl || null);
    }
  }, [initial]);

  const handleImageFile = (file) => {
    if (!file) return;
    setImageFile(file);
    setImagePreview(URL.createObjectURL(file));
  };

  const handleDrop = (e) => {
    e.preventDefault(); setDragOver(false);
    const file = e.dataTransfer.files?.[0];
    if (file && file.type.startsWith('image/')) handleImageFile(file);
  };

  useEffect(() => {
    return () => {
      if (imagePreview && imagePreview.startsWith('blob:')) URL.revokeObjectURL(imagePreview);
    };
  }, [imagePreview]);

  const handleSubmit = (e) => {
    e.preventDefault();
    const fd = new FormData();

    if (!isEdit) {
      fd.append('Name', form.name);
      fd.append('Description', form.description);
    }

    fd.append('CenterLocation.Latitude', parseFloat(form.centerLocation.latitude) || 0);
    fd.append('CenterLocation.Longitude', parseFloat(form.centerLocation.longitude) || 0);
    fd.append('DisplayOrder', Number(form.displayOrder) || 1);

    if (imageFile) {
      fd.append('Image', imageFile);
    }

    onSubmit(fd);
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* Visual Identity */}
      <Section title="Visual Identity">
        <div
          onDragOver={e => { e.preventDefault(); setDragOver(true); }}
          onDragLeave={() => setDragOver(false)}
          onDrop={handleDrop}
          style={{
            border: `1.5px dashed ${dragOver ? 'var(--accent-btn)' : 'var(--border-dash)'}`,
            borderRadius: 12, padding: imagePreview ? 0 : '32px 16px',
            textAlign: 'center', overflow: 'hidden',
            background: dragOver ? 'var(--bg-accent)' : 'var(--bg-surface)',
            transition: '.2s', position: 'relative',
            minHeight: imagePreview ? 0 : 160,
          }}
        >
          {imagePreview ? (
            <div style={{ position: 'relative' }}>
              <img
                src={imagePreview} alt="City preview"
                style={{ width: '100%', height: 180, objectFit: 'cover', display: 'block' }}
              />
              <div style={{
                position: 'absolute', inset: 0,
                background: 'rgba(0,0,0,.35)',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                opacity: 0, transition: 'opacity .2s',
              }}
                onMouseOver={e => e.currentTarget.style.opacity = '1'}
                onMouseOut={e => e.currentTarget.style.opacity = '0'}
              >
                <button
                  type="button"
                  onClick={() => fileRef.current.click()}
                  style={{
                    padding: '8px 20px', borderRadius: 8,
                    background: 'rgba(255,255,255,.9)', color: 'var(--text)', border: 'none',
                    fontWeight: 700, fontSize: 13, cursor: 'pointer',
                  }}
                >
                  Change Image
                </button>
              </div>
            </div>
          ) : (
            <>
              <div style={{
                width: 40, height: 40, borderRadius: 10,
                background: 'var(--accent-btn)',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                margin: '0 auto 10px',
              }}>
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth="2.5">
                  <path d="M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4M17 8l-5-5-5 5M12 3v12"/>
                </svg>
              </div>
              <div style={{ fontSize: 14, fontWeight: 600, color: 'var(--text)', marginBottom: 4 }}>City Image</div>
              <div style={{ fontSize: 12, color: 'var(--text-muted)', marginBottom: 12 }}>
                Drag and drop or click to select. JPG, PNG
              </div>
              <button
                type="button"
                onClick={() => fileRef.current.click()}
                style={{
                  padding: '8px 22px', borderRadius: 8,
                  background: 'var(--accent-btn)', color: '#fff', border: 'none',
                  fontWeight: 600, fontSize: 13, cursor: 'pointer',
                }}
              >
                Select File
              </button>
            </>
          )}
          <input
            ref={fileRef} type="file" accept="image/*" style={{ display: 'none' }}
            onChange={e => handleImageFile(e.target.files?.[0])}
          />
        </div>
      </Section>

      {/* Basic Details */}
      <Section title="Basic Details">
        {!isEdit && (
          <div className="form-group">
            <label>City Name *</label>
            <input
              required value={form.name}
              onChange={e => set('name', e.target.value)}
              placeholder="e.g. Cairo"
            />
          </div>
        )}

        {!isEdit && (
          <div className="form-group">
            <label>Description</label>
            <textarea
              value={form.description}
              onChange={e => set('description', e.target.value)}
              placeholder="Brief description of the city…"
              rows={3}
            />
          </div>
        )}
      </Section>

      {/* Geographic Configuration */}
      <Section title="Geographic Configuration">
        <div className="form-row">
          <div className="form-group">
            <label>Center Latitude *</label>
            <input
              type="number" step="any" required
              value={form.centerLocation.latitude}
              onChange={e => set('centerLocation.latitude', e.target.value)}
              placeholder="e.g. 30.0444"
            />
          </div>
          <div className="form-group">
            <label>Center Longitude *</label>
            <input
              type="number" step="any" required
              value={form.centerLocation.longitude}
              onChange={e => set('centerLocation.longitude', e.target.value)}
              placeholder="e.g. 31.2357"
            />
          </div>
        </div>
      </Section>

      {/* Display Settings */}
      <Section title="Display Settings">
        <div className="form-group" style={{ maxWidth: 200 }}>
          <label>Display Order</label>
          <input
            type="number" min="1"
            value={form.displayOrder}
            onChange={e => set('displayOrder', e.target.value)}
            placeholder="1"
          />
        </div>
      </Section>

      {/* Actions */}
      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 8 }}>
        <Button type="button" variant="secondary" onClick={onCancel} disabled={loading}>
          Cancel
        </Button>
        <Button type="submit" loading={loading}>
          {isEdit ? 'Save Changes' : 'Create City'}
        </Button>
      </div>
    </form>
  );
}
