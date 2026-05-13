import React from 'react';
import Modal from './Modal';
import Button from './Button';

export default function ConfirmDialog({
  open, onClose, onConfirm,
  title = 'Confirm Action',
  message = 'Are you sure you want to proceed?',
  confirmLabel = 'Confirm',
  variant = 'danger',
  loading = false,
}) {
  return (
    <Modal open={open} onClose={onClose} title={title} width={420}>
      <p style={{ color: 'var(--text-secondary)', marginBottom: 24, lineHeight: 1.7 }}>
        {message}
      </p>
      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
        <Button variant="secondary" onClick={onClose} disabled={loading}>
          Cancel
        </Button>
        <Button variant={variant} onClick={onConfirm} loading={loading}>
          {confirmLabel}
        </Button>
      </div>
    </Modal>
  );
}
