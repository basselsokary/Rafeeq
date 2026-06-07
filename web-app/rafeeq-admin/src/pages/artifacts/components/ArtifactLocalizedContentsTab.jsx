import React, { useState, useEffect } from 'react';
import { getLocalizedContents, addLocalizedContents, updateLocalizedContents } from '../../../api/artifactsApi';
import Button from '../../../components/common/Button';
import Spinner from '../../../components/common/Spinner';
import Badge from '../../../components/common/Badge';
import { LANGUAGE_CODES, formatEnum } from '../../../utils/constants';
import { useToast } from '../../../components/common/Toast';

const LANG_COLOR = { arabic: 'gold', english: 'blue', german: 'teal', russian: 'red', french: 'blue', spanish: 'green' };

export default function ArtifactLocalizedContentsTab({ artifactId }) {
  const toast = useToast();
  const [contents, setContents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [selected, setSelected] = useState(null);
  const [form, setForm] = useState({ language: 'english', name: '', description: ''});
  const [mode, setMode] = useState(null);

  const load = async () => {
    try {
      setLoading(true);
      const res = await getLocalizedContents(artifactId);
      setContents(Array.isArray(res.data) ? res.data : res.data?.value ?? []);
    } catch {
      toast('Failed to load localized contents', 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [artifactId]);

  const startAdd = () => {
    setForm({ language: 'english', name: '', description: '' });
    setMode('add');
    setSelected(null);
  };

  const startEdit = (c) => {
    setForm({
      language: c.language,
      name: c.name || '',
      description: c.description || '',
    });
    setSelected(c);
    setMode('edit');
  };

  const save = async () => {
    setSaving(true);
    try {
      if (mode === 'add') {
        await addLocalizedContents(artifactId, [form]);
        toast('Localized content added', 'success');
      } else {
        await updateLocalizedContents(artifactId, [{ ...form, id: selected.id }]);
        toast('Localized content updated', 'success');
      }
      setMode(null);
      load();
    } catch {
      toast('Save failed', 'error');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
        <h3 style={{ fontFamily: 'var(--font-display)', fontSize: 15, color: 'var(--text-secondary)' }}>
          {contents.length} language{contents.length !== 1 ? 's' : ''} configured
        </h3>
        <Button size="sm" onClick={startAdd}
          icon={<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>}>
          Add Language
        </Button>
      </div>

      {loading ? <Spinner center /> : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          {contents.map((c) => (
            <div key={c.id} style={{
              background: 'var(--bg-elevated)', border: '1px solid var(--border)',
              borderRadius: 'var(--radius-md)', padding: '14px 16px',
              display: 'flex', alignItems: 'flex-start', gap: 14,
            }}>
              <Badge color={LANG_COLOR[c.language] || 'gray'}>{formatEnum(c.language)}</Badge>
              <div style={{ flex: 1 }}>
                <div style={{ fontWeight: 600, marginBottom: 2 }}>{c.name || '(no name)'}</div>
                {c.description && (
                  <div style={{ color: 'var(--text-secondary)', fontSize: 12, lineHeight: 1.5 }}>
                    {c.description.slice(0, 120)}{c.description.length > 120 ? '…' : ''}
                  </div>
                )}
              </div>
              <Button size="sm" variant="ghost" onClick={() => startEdit(c)}>Edit</Button>
            </div>
          ))}
          {contents.length === 0 && (
            <div className="empty-state">
              <p>No localized content yet. Add a language to get started.</p>
            </div>
          )}
        </div>
      )}

      {/* Inline form */}
      {mode && (
        <div style={{
          marginTop: 24,
          background: 'var(--bg-elevated)',
          border: '1px solid var(--border)',
          borderRadius: 'var(--radius-md)',
          padding: 20,
        }}>
          <h4 style={{ marginBottom: 16, color: 'var(--gold)', fontFamily: 'var(--font-display)' }}>
            {mode === 'add' ? 'Add Localized Content' : 'Edit Localized Content'}
          </h4>

          <div className="form-row">
            <div className="form-group">
              <label>Language *</label>
              <select value={form.language} onChange={(e) => setForm(f => ({ ...f, language: e.target.value }))} disabled={mode === 'edit'}>
                {LANGUAGE_CODES.map(l => <option key={l} value={l}>{formatEnum(l)}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Name</label>
              <input value={form.name} onChange={(e) => setForm(f => ({ ...f, name: e.target.value }))} />
            </div>
          </div>

          <div className="form-group">
            <label>Description</label>
            <textarea value={form.description} onChange={(e) => setForm(f => ({ ...f, description: e.target.value }))} rows={3} />
          </div>

          <div style={{ display: 'flex', gap: 10, justifyContent: 'flex-end' }}>
            <Button variant="secondary" size="sm" onClick={() => setMode(null)} disabled={saving}>Cancel</Button>
            <Button size="sm" onClick={save} loading={saving}>Save</Button>
          </div>
        </div>
      )}
    </div>
  );
}
