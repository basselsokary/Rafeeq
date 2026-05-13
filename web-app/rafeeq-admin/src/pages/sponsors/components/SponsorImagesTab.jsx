import React, { useCallback, useEffect, useRef, useState } from 'react';
import { deleteSponsorImages, getSponsorImages, setMainSponsorImage, uploadSponsorImages } from '../../../api/sponsorsApi';
import Button from '../../../components/common/Button';
import Spinner from '../../../components/common/Spinner';
import ConfirmDialog from '../../../components/common/ConfirmDialog';
import { useToast } from '../../../components/common/Toast';

export default function SponsorImagesTab({ sponsorId }) {
  const toast = useToast();
  const fileRef = useRef(null);
  const MAX_UPLOAD = 5;
  const FALLBACK_IMAGE =
    "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='320' height='200' viewBox='0 0 320 200'><rect width='320' height='200' fill='%23e5e7eb'/><text x='50%25' y='50%25' font-size='14' fill='%236b7280' text-anchor='middle' dy='.3em'>Image unavailable</text></svg>";

  const [images, setImages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [deleteIds, setDeleteIds] = useState([]);
  const [deleting, setDeleting] = useState(false);
  const [staged, setStaged] = useState([]);
  const [selected, setSelected] = useState(null);
  const [selectedIds, setSelectedIds] = useState([]);
  const [dragOver, setDragOver] = useState(false);

  const load = useCallback(async () => {
    try {
      setLoading(true);
      const res = await getSponsorImages(sponsorId);
      const arr = Array.isArray(res.data) ? res.data : res.data?.value ?? [];
      setImages(arr);
    } catch {
      toast('Failed to load images', 'error');
    } finally {
      setLoading(false);
    }
  }, [sponsorId]);

  useEffect(() => {
    if (!sponsorId) return;
    load();
  }, [load, sponsorId]);

  useEffect(() => {
    setSelectedIds((prev) => prev.filter((id) => images.some((img) => img.id === id)));
    if (selected && !images.some((img) => img.id === selected.id)) setSelected(null);
  }, [images, selected]);

  const selectImage = (img) => {
    setSelected(img);
  };

  const handleFiles = (files) => {
    const f = Array.from(files);
    const limited = f.slice(0, MAX_UPLOAD);
    if (f.length > MAX_UPLOAD) toast(`Only ${MAX_UPLOAD} images allowed per upload.`, 'info');
    setStaged(
      limited.map((file, i) => ({
        file,
        isMain: images.length === 0 && i === 0,
        displayOrder: images.length + i + 1,
        preview: URL.createObjectURL(file),
        caption: '',
      })),
    );
  };

  const setMainImage = (index) => {
    setStaged((prev) => prev.map((s, i) => ({ ...s, isMain: i === index })));
  };

  const setCaption = (index, value) => {
    setStaged((prev) => prev.map((s, i) => (i === index ? { ...s, caption: value } : s)));
  };

  useEffect(() => {
    return () => {
      staged.forEach((s) => URL.revokeObjectURL(s.preview));
    };
  }, [staged]);

  const handleDrop = (e) => {
    e.preventDefault();
    setDragOver(false);
    handleFiles(e.dataTransfer.files);
  };

  const uploadAll = async () => {
    if (!staged.length) return;
    setUploading(true);
    try {
      const fd = new FormData();
      staged.forEach((s, i) => {
        fd.append(`Images[${i}].Image`, s.file);
        fd.append(`Images[${i}].IsMain`, String(s.isMain));
        fd.append(`Images[${i}].DisplayOrder`, String(s.displayOrder));
        if (s.caption) fd.append(`Images[${i}].Caption`, s.caption);
      });
      await uploadSponsorImages(sponsorId, fd);
      toast('Images uploaded', 'success');
      setStaged([]);
      load();
    } catch {
      toast('Upload failed', 'error');
    } finally {
      setUploading(false);
    }
  };

  const handleDelete = async (img) => {
    setDeleteIds([img.id]);
  };

  const confirmDelete = async () => {
    if (!deleteIds.length) return;
    setDeleting(true);
    try {
      await deleteSponsorImages(sponsorId, deleteIds);
      toast(deleteIds.length > 1 ? 'Images deleted' : 'Image deleted', 'success');
      setSelectedIds((prev) => prev.filter((id) => !deleteIds.includes(id)));
      if (selected?.id && deleteIds.includes(selected.id)) setSelected(null);
      setDeleteIds([]);
      load();
    } catch {
      toast('Delete failed', 'error');
    } finally {
      setDeleting(false);
    }
  };

  const toggleBulkSelection = (id) => {
    setSelectedIds((prev) => prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]);
  };

  const selectAll = () => setSelectedIds(images.map((img) => img.id));
  const clearSelection = () => setSelectedIds([]);

  const handleSetMainExisting = async (imageId) => {
    try {
      await setMainSponsorImage(sponsorId, imageId);
      toast('Main image updated', 'success');
      load();
    } catch {
      toast('Failed to set main image', 'error');
    }
  };

  return (
    <div>
      {/* Drop zone */}
      <div
        onDragOver={(e) => {
          e.preventDefault();
          setDragOver(true);
        }}
        onDragLeave={() => setDragOver(false)}
        onDrop={handleDrop}
        style={{
          border: `1.5px dashed ${dragOver ? 'var(--accent-btn)' : 'var(--border-dash)'}`,
          borderRadius: 12,
          padding: '40px 20px',
          textAlign: 'center',
          marginBottom: 32,
          background: dragOver ? 'var(--bg-accent)' : 'var(--bg-surface)',
          transition: '.2s',
        }}
      >
        <div
          style={{
            width: 48,
            height: 48,
            borderRadius: 10,
            background: 'var(--accent-btn)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            margin: '0 auto 14px',
          }}
        >
          <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth="2.5">
            <path d="M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4M17 8l-5-5-5 5M12 3v12" />
          </svg>
        </div>
        <div style={{ fontSize: 16, fontWeight: 600, color: 'var(--text)', marginBottom: 6 }}>Upload Sponsor Media</div>
        <div style={{ fontSize: 13, color: 'var(--text-muted)', marginBottom: 18 }}>
          Drag and drop your images here, or click to browse
          <br />
          files from your computer.
        </div>
        <button
          onClick={() => fileRef.current.click()}
          style={{
            padding: '10px 28px',
            background: 'var(--accent-btn)',
            color: '#fff',
            border: 'none',
            borderRadius: 8,
            fontSize: 14,
            fontWeight: 600,
            cursor: 'pointer',
          }}
        >
          Select Files
        </button>
        <input
          ref={fileRef}
          type="file"
          accept="image/*"
          multiple
          style={{ display: 'none' }}
          onChange={(e) => handleFiles(e.target.files)}
        />
        <div
          style={{
            marginTop: 14,
            fontSize: 11,
            color: 'var(--text-muted)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            gap: 10,
          }}
        >
          <span>JPG, PNG, WEBP</span>
          <span
            style={{
              width: 3,
              height: 3,
              borderRadius: '50%',
              background: 'var(--text-muted)',
              display: 'inline-block',
            }}
          />
          <span>MAX 5MB PER FILE</span>
        </div>
      </div>

      {/* Staged preview */}
      {staged.length > 0 && (
        <div style={{ marginBottom: 28 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
            <div style={{ fontSize: 13, fontWeight: 600, color: 'var(--accent)' }}>
              {staged.length} file{staged.length > 1 ? 's' : ''} ready to upload
            </div>
            <div style={{ display: 'flex', gap: 8 }}>
              <Button variant="ghost" size="sm" onClick={() => setStaged([])}>
                Discard
              </Button>
              <Button size="sm" onClick={uploadAll} loading={uploading}>
                Upload All
              </Button>
            </div>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill,minmax(140px,1fr))', gap: 10 }}>
            {staged.map((s, i) => (
              <div
                key={i}
                style={{
                  borderRadius: 8,
                  overflow: 'hidden',
                  position: 'relative',
                  background: 'var(--bg-base)',
                  border: '1px solid var(--border)',
                }}
              >
                <img src={s.preview} alt="" style={{ width: '100%', height: 100, objectFit: 'cover', display: 'block' }} />
                <button
                  type="button"
                  onClick={() => setMainImage(i)}
                  style={{
                    position: 'absolute',
                    top: 5,
                    left: 5,
                    background: s.isMain ? 'var(--accent-btn)' : 'rgba(0,0,0,.45)',
                    color: '#fff',
                    fontSize: 9,
                    fontWeight: 700,
                    padding: '2px 6px',
                    borderRadius: 10,
                    border: 'none',
                    cursor: 'pointer',
                  }}
                >
                  {s.isMain ? 'MAIN' : 'SET MAIN'}
                </button>
                <div style={{ padding: '8px 8px 10px' }}>
                  <input
                    value={s.caption}
                    onChange={(e) => setCaption(i, e.target.value)}
                    placeholder="Caption (optional)"
                    style={{ fontSize: 12, padding: '6px 8px' }}
                  />
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {loading ? (
        <Spinner center />
      ) : (
        <div>
          {images.length > 0 && (
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 }}>
              <div style={{ fontSize: 13, fontWeight: 600, color: 'var(--text)' }}>
                {selectedIds.length ? `${selectedIds.length} selected` : `${images.length} total`}
              </div>
              <div style={{ display: 'flex', gap: 8 }}>
                {selectedIds.length === images.length ? (
                  <Button variant="ghost" size="sm" onClick={clearSelection}>
                    Clear
                  </Button>
                ) : (
                  <Button variant="ghost" size="sm" onClick={selectAll}>
                    Select All
                  </Button>
                )}
                <Button variant="danger" size="sm" disabled={!selectedIds.length} onClick={() => setDeleteIds(selectedIds)}>
                  Delete Selected
                </Button>
              </div>
            </div>
          )}
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, minmax(0, 1fr))', gap: 16 }}>
            {images.map((img) => (
              <div
                key={img.id}
                onClick={() => selectImage(img)}
                style={{
                  position: 'relative',
                  borderRadius: 12,
                  overflow: 'hidden',
                  cursor: 'pointer',
                  border: `2px solid ${selected?.id === img.id ? 'var(--accent-btn)' : 'transparent'}`,
                  background: 'var(--bg-surface)',
                  boxShadow: '0 6px 18px rgba(6,10,18,0.06)',
                  transition: 'transform .15s, box-shadow .15s, border-color .15s',
                }}
                onMouseOver={(e) => {
                  e.currentTarget.style.transform = 'translateY(-2px)';
                }}
                onMouseOut={(e) => {
                  e.currentTarget.style.transform = 'translateY(0)';
                }}
              >
                <label style={{ position: 'absolute', top: 8, left: 8, zIndex: 2 }}>
                  <input
                    type="checkbox"
                    checked={selectedIds.includes(img.id)}
                    onChange={() => toggleBulkSelection(img.id)}
                    onClick={(e) => e.stopPropagation()}
                    style={{
                      width: 16,
                      height: 16,
                      accentColor: 'var(--accent-btn)',
                      background: 'rgba(11,15,26,.6)',
                      borderRadius: 4,
                      boxShadow: '0 2px 6px rgba(0,0,0,.25)',
                    }}
                  />
                </label>

                <img
                  src={img.url || img.imageUrl || ''}
                  alt={img.caption || ''}
                  style={{ width: '100%', height: 180, objectFit: 'cover', display: 'block' }}
                  loading="lazy"
                  decoding="async"
                  onError={(e) => {
                    if (e.currentTarget.src !== FALLBACK_IMAGE) {
                      e.currentTarget.src = FALLBACK_IMAGE;
                      e.currentTarget.style.objectFit = 'contain';
                      e.currentTarget.style.background = '#f3f4f6';
                    }
                  }}
                />

                {img.isMain && (
                  <span
                    style={{
                      position: 'absolute',
                      bottom: 8,
                      left: 8,
                      background: 'var(--gold)',
                      color: '#0b0f1a',
                      fontSize: 10,
                      fontWeight: 700,
                      padding: '2px 7px',
                      borderRadius: 10,
                    }}
                  >
                    MAIN
                  </span>
                )}

                {!img.isMain && (
                  <button
                    type="button"
                    title="Set as main"
                    onClick={(e) => {
                      e.stopPropagation();
                      handleSetMainExisting(img.id);
                    }}
                    style={{
                      position: 'absolute',
                      bottom: 8,
                      left: 8,
                      width: 28,
                      height: 28,
                      borderRadius: 8,
                      background: 'rgba(0,0,0,.5)',
                      border: 'none',
                      cursor: 'pointer',
                      color: '#fff',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      opacity: 0,
                    }}
                    onMouseOver={(e) => (e.currentTarget.style.opacity = '1')}
                    onMouseOut={(e) => (e.currentTarget.style.opacity = '0')}
                  >
                    ★
                  </button>
                )}

                <div style={{ padding: '8px 10px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <span style={{ fontSize: 11, color: 'var(--text-muted)' }}>{img.caption || `#${img.displayOrder ?? ''}`}</span>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      handleDelete(img);
                    }}
                    style={{
                      position: 'absolute',
                      top: 8,
                      right: 8,
                      width: 26,
                      height: 26,
                      borderRadius: 6,
                      background: 'rgba(0,0,0,.5)',
                      border: 'none',
                      cursor: 'pointer',
                      color: '#fff',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      opacity: 0,
                    }}
                    onMouseOver={(e) => (e.currentTarget.style.opacity = '1')}
                    onMouseOut={(e) => (e.currentTarget.style.opacity = '0')}
                    className="img-delete-btn"
                  >
                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                      <polyline points="3 6 5 6 21 6" />
                      <path d="M19 6l-1 14H6L5 6" />
                      <path d="M9 6V4h6v2" />
                    </svg>
                  </button>
                </div>
              </div>
            ))}
            {images.length === 0 && (
              <div className="empty-state">
                <p>No images uploaded yet.</p>
              </div>
            )}
          </div>
        </div>
      )}

      <ConfirmDialog
        open={deleteIds.length > 0}
        onClose={() => setDeleteIds([])}
        onConfirm={confirmDelete}
        loading={deleting}
        title={deleteIds.length > 1 ? 'Delete Images' : 'Delete Image'}
        message={deleteIds.length > 1 ? 'Permanently delete these images from the sponsor?' : 'Permanently delete this image from the sponsor?'}
        confirmLabel="Delete"
      />
    </div>
  );
}
