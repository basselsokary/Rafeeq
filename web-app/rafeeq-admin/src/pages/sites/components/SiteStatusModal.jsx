import React, { useState, useEffect } from 'react';
import Modal from '../../../components/common/Modal';
import Button from '../../../components/common/Button';
import { SITE_STATUSES, STATUS_LABELS, formatEnum } from '../../../utils/constants';

export default function SiteStatusModal({ open, onClose, site, onSave, loading }) {
  const [form, setForm] = useState({
    status: 'active',
    activate: true,
    isFeatured: false,
    isHiddenGem: false,
  });

  useEffect(() => {
    if (site) {
      setForm({
        status: site.status || 'active',
        activate: site.status === 'active',
        isFeatured: site.isFeatured || false,
        isHiddenGem: site.isHiddenGem || false,
      });
    }
  }, [site]);

  const STATUS_COLOR = {
    active: 'var(--status-active)',
    underMaintenance: 'var(--status-maintenance)',
    temporarilyClosed: 'var(--status-closed)',
    permanentlyClosed: 'var(--text-muted)',
  };

  return (
    <Modal open={open} onClose={onClose} title="Update Site Status" width={440}>
      <div style={{ marginBottom: 20 }}>
        <label>Status</label>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 8, marginTop: 8 }}>
          {SITE_STATUSES.map((s) => (
            <label
              key={s}
              style={{
                display: 'flex', alignItems: 'center', gap: 12,
                padding: '10px 14px',
                borderRadius: 'var(--radius-sm)',
                border: `1px solid ${form.status === s ? STATUS_COLOR[s] : 'var(--border)'}`,
                background: form.status === s ? 'rgba(255,255,255,0.03)' : 'transparent',
                cursor: 'pointer', transition: 'all var(--transition)',
              }}
            >
              <input
                type="radio" name="status" value={s}
                checked={form.status === s}
                onChange={() => setForm((f) => ({
                  ...f,
                  status: s,
                  activate: s === 'active',
                }))}
                style={{ accentColor: STATUS_COLOR[s], width: 15, height: 15, cursor: 'pointer' }}
              />
              <div>
                <div style={{ fontWeight: 500, color: STATUS_COLOR[s], fontSize: 13 }}>
                  {STATUS_LABELS[s]}
                </div>
              </div>
            </label>
          ))}
        </div>
      </div>

      <hr className="divider" />

      <div style={{ marginBottom: 20 }}>
        <label style={{ marginBottom: 12 }}>Flags</label>
        <label className="checkbox-group">
          <input
            type="checkbox" checked={form.isFeatured}
            onChange={(e) => setForm((f) => ({ ...f, isFeatured: e.target.checked }))}
          />
          <span>⭐ Featured site (shown prominently in the app)</span>
        </label>
        <label className="checkbox-group">
          <input
            type="checkbox" checked={form.isHiddenGem}
            onChange={(e) => setForm((f) => ({ ...f, isHiddenGem: e.target.checked }))}
          />
          <span>💎 Hidden gem (curated discovery)</span>
        </label>
      </div>

      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
        <Button variant="secondary" onClick={onClose} disabled={loading}>Cancel</Button>
        <Button onClick={() => onSave(form)} loading={loading}>Save Status</Button>
      </div>
    </Modal>
  );
}
