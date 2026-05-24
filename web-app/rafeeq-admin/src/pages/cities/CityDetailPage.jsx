import React, { useRef, useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getCityById, updateCity, deleteCity, updateCityImage } from '../../api/citiesApi';
import CityLocalizedContentsTab from './components/CityLocalizedContentsTab';
import Modal from '../../components/common/Modal';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import Spinner from '../../components/common/Spinner';
import CityForm from './components/CityForm';
import { useToast } from '../../components/common/Toast';

const TABS = [
  { id: 'basic',    label: 'Basic Info'   },
  { id: 'localize', label: 'Localization' },
];

function Btn({ children, onClick, variant = 'primary', size = 'md', loading = false, disabled = false, type = 'button', style: s = {} }) {
  const pad  = size === 'sm' ? '6px 14px' : '9px 20px';
  const fz   = size === 'sm' ? 12 : 13;
  const base = {
    display: 'inline-flex', alignItems: 'center', gap: 7,
    cursor: disabled || loading ? 'not-allowed' : 'pointer',
    border: 'none', borderRadius: size === 'sm' ? 8 : 10,
    fontFamily: 'var(--font-body)', fontWeight: 700, fontSize: fz, padding: pad,
    opacity: disabled ? 0.5 : 1, transition: 'all .15s', ...s,
  };
  const v = {
    primary:   { background: 'linear-gradient(135deg, var(--primary), var(--primary-container))', color: '#fff', boxShadow: '0 2px 8px rgba(124,87,45,.25)' },
    secondary: { background: 'var(--surface-container-lowest)', color: 'var(--text-2)', boxShadow: '0 0 0 1px rgba(212,196,183,.5)' },
    ghost:     { background: 'transparent', color: 'var(--text-2)' },
    danger:    { background: 'rgba(186,26,26,.07)', color: 'var(--error)', boxShadow: '0 0 0 1px rgba(186,26,26,.2)' },
  };
  return (
    <button type={type} onClick={onClick} disabled={disabled || loading} style={{ ...base, ...v[variant] }}>
      {loading ? <Spinner size={13} /> : null}{children}
    </button>
  );
}

function InfoRow({ label, value }) {
  if (value == null || value === '') return null;
  return (
    <div style={{
      display: 'flex', justifyContent: 'space-between', alignItems: 'baseline',
      padding: '11px 18px',
      borderBottom: '1px solid rgba(212,196,183,.2)',
    }}>
      <span style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--outline)', flexShrink: 0, marginRight: 20 }}>
        {label}
      </span>
      <span style={{ fontSize: 13, color: 'var(--text)', textAlign: 'right' }}>{String(value)}</span>
    </div>
  );
}

function BasicInfoTab({ city, cityId, onEdit, onImageUpdated, toast }) {
  const fileInputRef = useRef(null);
  const [selectedFile, setSelectedFile] = useState(null);
  const [uploading, setUploading] = useState(false);

  const pickImage = () => {
    if (uploading) return;
    fileInputRef.current?.click();
  };

  const onFileChange = (e) => {
    const file = e.target.files?.[0] || null;
    setSelectedFile(file);
  };

  const uploadImage = async () => {
    if (!selectedFile) return;
    setUploading(true);
    try {
      const fd = new FormData();
      fd.append('image', selectedFile);

      await updateCityImage(cityId, fd);
      toast?.('Image updated', 'success');
      setSelectedFile(null);
      if (fileInputRef.current) fileInputRef.current.value = '';
      onImageUpdated?.();
    } catch (e) {
      console.error('Image update failed:', e.response?.data || e);
      toast?.('Image update failed', 'error');
    } finally {
      setUploading(false);
    }
  };

  return (
    <div style={{ maxWidth: 720 }}>
      {/* Cover image */}
      <div style={{ borderRadius: 16, overflow: 'hidden', marginBottom: 20, background: 'var(--surface-container)', aspectRatio: '21/9', position: 'relative' }}>
        {city.imageUrl
          ? <img src={city.imageUrl} alt={city.name} style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }} />
          : <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--outline)', fontSize: 13 }}>No cover image</div>
        }
        <div style={{
          position: 'absolute', top: 0, right: 0,
          width: 0, height: 0, borderStyle: 'solid',
          borderWidth: '0 40px 40px 0',
          borderColor: 'transparent rgba(124,87,45,0.2) transparent transparent',
        }} />

        <div style={{ position: 'absolute', left: 12, bottom: 12, display: 'flex', gap: 8, alignItems: 'center' }}>
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            onChange={onFileChange}
            style={{ display: 'none' }}
          />
          <Btn size="sm" variant="secondary" onClick={pickImage} disabled={uploading}>
            Change image
          </Btn>
          {selectedFile ? (
            <Btn size="sm" onClick={uploadImage} loading={uploading}>
              Upload
            </Btn>
          ) : null}
        </div>
      </div>

      {selectedFile ? (
        <div style={{
          marginBottom: 20,
          background: 'var(--surface-container-lowest)',
          borderRadius: 12,
          padding: '12px 14px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          gap: 12,
          boxShadow: '0 1px 4px rgba(29,27,23,.05)',
        }}>
          <div style={{ minWidth: 0 }}>
            <div style={{ fontSize: 11, fontWeight: 800, letterSpacing: '0.06em', textTransform: 'uppercase', color: 'var(--outline)' }}>
              Selected image
            </div>
            <div style={{ fontSize: 13, color: 'var(--text)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
              {selectedFile.name}
            </div>
          </div>
          <div style={{ display: 'flex', gap: 8, flexShrink: 0 }}>
            <Btn
              size="sm"
              variant="ghost"
              onClick={() => {
                if (uploading) return;
                setSelectedFile(null);
                if (fileInputRef.current) fileInputRef.current.value = '';
              }}
              disabled={uploading}
            >
              Cancel
            </Btn>
            <Btn size="sm" onClick={uploadImage} loading={uploading} disabled={uploading}>
              Upload
            </Btn>
          </div>
        </div>
      ) : null}

      {/* Description */}
      {city.description && (
        <p style={{
          color: 'var(--text-2)', fontSize: 14, lineHeight: 1.75, marginBottom: 20,
          background: 'var(--surface-container-low)', borderRadius: 12, padding: '14px 18px',
        }}>{city.description}</p>
      )}

      {/* Info card */}
      <div style={{
        background: 'var(--surface-container-lowest)', borderRadius: 14, overflow: 'hidden', marginBottom: 20,
        boxShadow: '0 1px 4px rgba(29,27,23,.05)',
      }}>
        <InfoRow label="Coordinates" value={city.centerLocation ? `${city.centerLocation.latitude}, ${city.centerLocation.longitude}` : null} />
        <InfoRow label="Display Order" value={city.displayOrder} />
        <InfoRow label="Total Sites" value={city.totalSites ?? 0} />
        {city.createdAt && (
          <InfoRow label="Created" value={new Date(city.createdAt).toLocaleString()} />
        )}
        {city.updatedAt && (
          <InfoRow label="Last Modified" value={new Date(city.updatedAt).toLocaleString()} />
        )}
        <div style={{ height: 1 }} />
      </div>

      <Btn onClick={onEdit}>
        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
          <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4z"/>
        </svg>
        Edit City
      </Btn>
    </div>
  );
}

export default function CityDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const toast = useToast();

  const [city,     setCity]     = useState(null);
  const [loading,  setLoading]  = useState(true);
  const [tab,      setTab]      = useState('basic');
  const [editOpen, setEditOpen] = useState(false);
  const [delOpen,  setDelOpen]  = useState(false);
  const [saving,   setSaving]   = useState(false);
  const [deleting, setDeleting] = useState(false);

  const load = useCallback(async () => {
    try {
      setLoading(true);
      const res = await getCityById(id);
      setCity(res.data?.value ?? res.data);
    } catch { toast('Failed to load city', 'error'); }
    finally { setLoading(false); }
  }, [id, toast]);

  useEffect(() => { load(); }, [load]);

  const handleUpdate = async (formData) => {
    setSaving(true);
    try {
      await updateCity(id, formData);
      toast('Saved', 'success');
      setEditOpen(false);
      load();
    } catch (e) {
      console.error('Update failed:', e.response?.data || e);
      toast('Update failed', 'error');
    } finally { setSaving(false); }
  };

  const handleDelete = async () => {
    setDeleting(true);
    try { await deleteCity(id); toast('City deleted', 'success'); navigate('/cities'); }
    catch { toast('Delete failed', 'error'); }
    finally { setDeleting(false); }
  };

  if (loading) return (
    <div style={{ minHeight: '100vh', background: 'var(--background)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <Spinner size={36} />
    </div>
  );

  if (!city) return (
    <div style={{ minHeight: '100vh', background: 'var(--background)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--outline)' }}>
      City not found.
    </div>
  );

  return (
    <div style={{ minHeight: '100vh', background: 'var(--background)', fontFamily: 'var(--font-body)', display: 'flex', flexDirection: 'column' }}>

      <div style={{ padding: '24px 32px 0', display: 'flex', alignItems: 'center', gap: 20, flexWrap: 'wrap' }}>
        <button onClick={() => navigate('/cities')} style={{
          display: 'flex', alignItems: 'center', gap: 6,
          background: 'none', border: 'none', cursor: 'pointer',
          color: 'var(--text-2)', fontSize: 13, fontWeight: 500, padding: 0,
        }}>
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <path d="M19 12H5M12 5l-7 7 7 7"/>
          </svg>
          Back to Cities
        </button>

        <div style={{ width: 1, height: 24, background: 'rgba(212,196,183,.5)' }} />

        <div style={{ display: 'flex', alignItems: 'center', gap: 10, flex: 1, minWidth: 0 }}>
          <span style={{ fontFamily: 'var(--font-display)', fontWeight: 700, fontSize: 16, color: 'var(--text)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
            Edit City: {city.name || 'Unnamed'}
          </span>
          <span style={{
            fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase',
            padding: '3px 9px', borderRadius: 4, flexShrink: 0,
            background: 'rgba(124,87,45,.08)', color: 'var(--primary)',
          }}>
            #{city.displayOrder ?? '—'}
          </span>
        </div>

        <div style={{ display: 'flex', gap: 8, flexShrink: 0 }}>
          <Btn variant="danger" size="sm" onClick={() => setDelOpen(true)}>Delete</Btn>
        </div>
      </div>

      {/* Tab bar */}
      <div style={{
        background: 'var(--topbar-bg)', backdropFilter: 'blur(12px)',
        borderBottom: '1px solid var(--topbar-border)',
        padding: '0 32px', display: 'flex', gap: 2,
        position: 'sticky', top: 64, zIndex: 49,
      }}>
        {TABS.map(t => (
          <button key={t.id} onClick={() => setTab(t.id)} style={{
            padding: '13px 16px', border: 'none', background: 'none', cursor: 'pointer',
            fontSize: 13, fontWeight: tab === t.id ? 700 : 500,
            color: tab === t.id ? 'var(--primary)' : 'var(--text-2)',
            borderBottom: `2px solid ${tab === t.id ? 'var(--primary)' : 'transparent'}`,
            marginBottom: -1, transition: 'all .15s',
          }}>
            {t.label}
          </button>
        ))}
      </div>

      {/* Content */}
      <div style={{ flex: 1, padding: '32px', maxWidth: 1000, width: '100%', margin: '0 auto', boxSizing: 'border-box' }}>
        {tab === 'basic'    && (
          <BasicInfoTab
            city={city}
            cityId={id}
            onEdit={() => setEditOpen(true)}
            onImageUpdated={load}
            toast={toast}
          />
        )}
        {tab === 'localize' && <CityLocalizedContentsTab cityId={id} />}
      </div>

      {/* Modals */}
      <Modal open={editOpen} onClose={() => setEditOpen(false)} title="Edit City" width={640}>
        <CityForm initial={city} onSubmit={handleUpdate} loading={saving} onCancel={() => setEditOpen(false)} />
      </Modal>

      <ConfirmDialog
        open={delOpen} onClose={() => setDelOpen(false)}
        onConfirm={handleDelete} loading={deleting}
        title="Delete City"
        message={`Permanently delete "${city.name || 'this city'}"? This cannot be undone.`}
        confirmLabel="Delete City"
      />
    </div>
  );
}
