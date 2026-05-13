import React, { useState, useEffect } from 'react';
import Spinner from '../../../components/common/Spinner';

const TYPES = ['restaurant','hotel','shop','service','tour','transportation'];
const TIERS = ['bronze','silver','gold','platinum'];

const fmt = v => v ? v.replace(/([A-Z])/g,' $1').replace(/^./,s=>s.toUpperCase()) : '';

const today     = () => new Date().toISOString().slice(0,10);
const oneYear   = () => { const d = new Date(); d.setFullYear(d.getFullYear()+1); return d.toISOString().slice(0,10); };

const DEFAULTS = {
  title: '', description: '', address: '',
  type: 'restaurant', tier: 'bronze',
  location: { latitude: '', longitude: '' },
  startDate: today(), endDate: oneYear(),
  websiteUrl: '', contactPhone: '', contactEmail: '',
};

function Btn({ children, type='button', onClick, variant='primary', loading=false, disabled=false }) {
  const v = {
    primary:   { background:'linear-gradient(135deg,var(--primary),var(--primary-container))', color:'#fff', boxShadow:'0 2px 8px rgba(124,87,45,.2)', border:'none' },
    secondary: { background:'var(--surface-container-lowest)', color:'var(--text-2)', boxShadow:'0 0 0 1px rgba(212,196,183,.5)', border:'none' },
    ghost:     { background:'transparent', color:'var(--text-2)', border:'none' },
  };
  return (
    <button type={type} onClick={onClick} disabled={disabled||loading} style={{ display:'inline-flex', alignItems:'center', gap:7, padding:'9px 20px', borderRadius:10, fontSize:13, fontWeight:700, cursor:disabled||loading?'not-allowed':'pointer', opacity:disabled?0.5:1, transition:'all .15s', fontFamily:'var(--font-body)', ...v[variant] }}>
      {loading && <Spinner size={13} />}{children}
    </button>
  );
}

function Field({ label, children, note }) {
  return (
    <div style={{ marginBottom: 18 }}>
      <div style={{ display:'flex', justifyContent:'space-between', marginBottom:6 }}>
        <label style={{ fontSize:10, fontWeight:700, letterSpacing:'0.08em', textTransform:'uppercase', color:'var(--outline)' }}>{label}</label>
        {note && <span style={{ fontSize:11, color:'var(--outline)' }}>{note}</span>}
      </div>
      {children}
    </div>
  );
}

const inputStyle = { width:'100%', padding:'10px 14px', fontSize:14, color:'var(--text)', background:'var(--surface-container-low)', border:'1px solid rgba(212,196,183,.3)', borderRadius:10, outline:'none', fontFamily:'var(--font-body)' };
const selectStyle = { ...inputStyle, appearance:'none', backgroundImage:`url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%23827569' stroke-width='2'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E")`, backgroundRepeat:'no-repeat', backgroundPosition:'right 12px center', paddingRight:36 };

function Section({ title, children }) {
  return (
    <div style={{ marginBottom:24 }}>
      <div style={{ fontSize:12, fontWeight:700, letterSpacing:'0.08em', textTransform:'uppercase', color:'var(--primary)', marginBottom:14, paddingBottom:8, borderBottom:'1px solid rgba(212,196,183,.3)' }}>{title}</div>
      {children}
    </div>
  );
}

export default function SponsorForm({ initial = null, onSubmit, loading, onCancel }) {
  const [form, setForm] = useState(() => initial ? { ...DEFAULTS, ...initial } : DEFAULTS);

  useEffect(() => { if (initial) setForm({ ...DEFAULTS, ...initial }); }, [initial]);

  const set = (path, value) => setForm(prev => {
    const clone = JSON.parse(JSON.stringify(prev));
    const keys  = path.split('.');
    let obj = clone;
    for (let i = 0; i < keys.length - 1; i++) obj = obj[keys[i]];
    obj[keys[keys.length - 1]] = value;
    return clone;
  });

  const handleSubmit = (e) => {
    e.preventDefault();
    const payload = {
      ...form,
      location: {
        latitude:  parseFloat(form.location.latitude),
        longitude: parseFloat(form.location.longitude),
      },
    };
    ['title','description','address','websiteUrl','contactPhone','contactEmail'].forEach(k => {
      if (!payload[k]) delete payload[k];
    });
    onSubmit(payload);
  };

  const isEdit = !!initial;

  return (
    <form onSubmit={handleSubmit}>
      <Section title="Basic Information">
        <Field label="Title">
          <input value={form.title} onChange={e => set('title', e.target.value)} placeholder="e.g. Koshary El Tahrir" style={inputStyle} />
        </Field>
        <Field label="Description">
          <textarea value={form.description} onChange={e => set('description', e.target.value)} rows={3} placeholder="Brief description…" style={{ ...inputStyle, resize:'vertical', minHeight:70 }} />
        </Field>
        <div style={{ display:'grid', gridTemplateColumns:'1fr 1fr', gap:16 }}>
          <Field label="Type *">
            <select value={form.type} onChange={e => set('type', e.target.value)} required style={selectStyle}>
              {TYPES.map(t => <option key={t} value={t}>{fmt(t)}</option>)}
            </select>
          </Field>
          <Field label="Tier *">
            <select value={form.tier} onChange={e => set('tier', e.target.value)} required style={selectStyle}>
              {TIERS.map(t => <option key={t} value={t}>{fmt(t)}</option>)}
            </select>
          </Field>
        </div>
        <Field label="Address">
          <input value={form.address} onChange={e => set('address', e.target.value)} placeholder="Street / district" style={inputStyle} />
        </Field>
      </Section>

      <Section title="Location">
        <div style={{ display:'grid', gridTemplateColumns:'1fr 1fr', gap:16 }}>
          <Field label="Latitude *">
            <input type="number" step="any" required value={form.location.latitude} onChange={e => set('location.latitude', e.target.value)} placeholder="e.g. 30.0444" style={inputStyle} />
          </Field>
          <Field label="Longitude *">
            <input type="number" step="any" required value={form.location.longitude} onChange={e => set('location.longitude', e.target.value)} placeholder="e.g. 31.2357" style={inputStyle} />
          </Field>
        </div>
      </Section>

      <Section title="Contract Period">
        <div style={{ display:'grid', gridTemplateColumns:'1fr 1fr', gap:16 }}>
          {!isEdit && (
            <Field label="Start Date *">
              <input type="date" required value={form.startDate} onChange={e => set('startDate', e.target.value)} style={inputStyle} />
            </Field>
          )}
          <Field label={isEdit ? 'New End Date' : 'End Date *'}>
            <input type="date" required={!isEdit} value={isEdit ? (form.newEndDate || '') : form.endDate}
              onChange={e => set(isEdit ? 'newEndDate' : 'endDate', e.target.value)} style={inputStyle} />
          </Field>
        </div>
      </Section>

      <Section title="Contact Information">
        <div style={{ display:'grid', gridTemplateColumns:'1fr 1fr', gap:16 }}>
          <Field label="Phone">
            <input value={form.contactPhone} onChange={e => set('contactPhone', e.target.value)} placeholder="+20 2 XXXX XXXX" style={inputStyle} />
          </Field>
          <Field label="Email">
            <input type="email" value={form.contactEmail} onChange={e => set('contactEmail', e.target.value)} placeholder="sponsor@example.com" style={inputStyle} />
          </Field>
        </div>
        <Field label="Website URL">
          <input type="url" value={form.websiteUrl} onChange={e => set('websiteUrl', e.target.value)} placeholder="https://…" style={inputStyle} />
        </Field>
      </Section>

      <div style={{ display:'flex', justifyContent:'flex-end', gap:10, marginTop:8 }}>
        <Btn variant="ghost" onClick={onCancel} disabled={loading}>Cancel</Btn>
        <Btn type="submit" loading={loading}>{isEdit ? 'Save Changes' : 'Create Sponsor'}</Btn>
      </div>
    </form>
  );
}
