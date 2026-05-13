import React from 'react';
import Spinner from './Spinner';

export default function Table({ columns, data, loading, onRowClick, emptyText = 'No records found.' }) {
  return (
    <div style={{
      background: 'var(--bg-card)',
      border: '1px solid var(--border)',
      borderRadius: 'var(--radius-md)',
      overflow: 'hidden',
    }}>
      <div style={{ overflowX: 'auto' }}>
        <table style={{
          width: '100%', borderCollapse: 'collapse',
          fontSize: 13, fontFamily: 'var(--font-body)',
        }}>
          <thead>
            <tr style={{ background: 'var(--bg-elevated)' }}>
              {columns.map((col) => (
                <th
                  key={col.key}
                  style={{
                    padding: '11px 16px',
                    textAlign: col.align || 'left',
                    fontSize: 11, fontWeight: 600,
                    letterSpacing: '0.06em', textTransform: 'uppercase',
                    color: 'var(--text-muted)',
                    borderBottom: '1px solid var(--border)',
                    whiteSpace: 'nowrap',
                    width: col.width,
                  }}
                >
                  {col.label}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={columns.length} style={{ padding: '48px', textAlign: 'center' }}>
                  <Spinner size={28} center />
                </td>
              </tr>
            ) : data.length === 0 ? (
              <tr>
                <td
                  colSpan={columns.length}
                  style={{ padding: '48px', textAlign: 'center', color: 'var(--text-muted)' }}
                >
                  {emptyText}
                </td>
              </tr>
            ) : (
              data.map((row, ri) => (
                <tr
                  key={row.id || ri}
                  onClick={() => onRowClick && onRowClick(row)}
                  style={{
                    borderBottom: ri < data.length - 1 ? '1px solid var(--border)' : 'none',
                    cursor: onRowClick ? 'pointer' : 'default',
                    transition: 'background var(--transition)',
                  }}
                  onMouseOver={(e) => {
                    if (onRowClick) e.currentTarget.style.background = 'var(--bg-elevated)';
                  }}
                  onMouseOut={(e) => {
                    e.currentTarget.style.background = 'transparent';
                  }}
                >
                  {columns.map((col) => (
                    <td
                      key={col.key}
                      style={{
                        padding: '12px 16px',
                        color: 'var(--text-primary)',
                        textAlign: col.align || 'left',
                        verticalAlign: 'middle',
                      }}
                    >
                      {col.render ? col.render(row[col.key], row) : (row[col.key] ?? '—')}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
