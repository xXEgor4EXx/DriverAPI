// Конфиг
const API = 'http://localhost:5122';

// Переменные состояния
let accessToken = '';
let refreshToken = '';
let currentCtrl = '';
let currentData = [];

// Определения полей для каждого контроллера
const FIELDS = {
  CntrEmployers: [
    { key: 'FullName', label: 'ФИО (Фамилия Имя Отчество)', type: 'text', placeholder: 'Иванов Иван Иванович' },
    { key: 'Phone', label: 'Телефон (11 цифр)', type: 'text', placeholder: '79001234567' }
  ],
  CntrProfessions: [
    { key: 'Title', label: 'Название профессии', type: 'text', placeholder: 'Токарь' }
  ],
  CntrOperations: [
    { key: 'Description', label: 'Описание операции', type: 'text', placeholder: 'Обточка вала' },
    { key: 'RatePerUnit', label: 'Расценка за единицу (₽)', type: 'number', placeholder: '150.00' }
  ],
  CntrWorks: [
    {
      key: 'EmployeeID', label: 'Сотрудник', type: 'select',
      source: 'CntrEmployers', valueKey: 'employeeID',
      labelFn: r => `[${r.employeeID}] ${r.fullName}`
    },
    {
      key: 'OperationID', label: 'Операция', type: 'select',
      source: 'CntrOperations', valueKey: 'operationID',
      labelFn: r => `[${r.operationID}] ${r.description} — ${r.ratePerUnit} ₽/ед.`
    },
    { key: 'WorkDate', label: 'Дата работы', type: 'date' },
    { key: 'Quantity', label: 'Количество выполнено', type: 'number', placeholder: '100' },
    { key: 'RejectedQuantity', label: 'Бракованных деталей', type: 'number', placeholder: '0' }
  ],
  CntrAccrualsType: [
    { key: 'Name', label: 'Название типа', type: 'text', placeholder: 'Основной' },
    { key: 'position', label: 'Позиция (порядок)', type: 'number', placeholder: '1' }
  ],
  CntrAccruals: [
    {
      key: 'AccrualsTypeID', label: 'Тип начисления', type: 'select',
      source: 'CntrAccrualsType', valueKey: 'accrualsTypeID',
      labelFn: r => `[${r.accrualsTypeID}] ${r.name}`
    },
    {
      key: 'WorkID', label: 'Наряд / Работа', type: 'select',
      source: 'CntrWorks', valueKey: 'workID',
      labelFn: r => `[${r.workID}] Сотр.${r.employeeID} — ${r.workDate ? new Date(r.workDate).toLocaleDateString('ru-RU') : ''}`
    },
    { key: 'Bonus', label: 'Премия (₽)', type: 'number', placeholder: '0' },
    { key: 'AccrualTotal', label: 'Сумма начисления (₽)', type: 'number', placeholder: '0' }
  ],
  CntrPayments: [
    {
      key: 'EmployeeID', label: 'Сотрудник', type: 'select',
      source: 'CntrEmployers', valueKey: 'employeeID',
      labelFn: r => `[${r.employeeID}] ${r.fullName}`
    },
    { key: 'AmountToPay', label: 'Сумма к выдаче (₽)', type: 'number', placeholder: '0' },
    { key: 'PaymentDate', label: 'Дата выплаты', type: 'date' }
  ],
  CntrProfessionEmployer: [
    {
      key: 'EmployeeID', label: 'Сотрудник', type: 'select',
      source: 'CntrEmployers', valueKey: 'employeeID',
      labelFn: r => `[${r.employeeID}] ${r.fullName}`
    },
    {
      key: 'ProfessionID', label: 'Профессия', type: 'select',
      source: 'CntrProfessions', valueKey: 'professionID',
      labelFn: r => `[${r.professionID}] ${r.title}`
    },
    { key: 'Name', label: 'Примечание', type: 'text', placeholder: 'Основная должность' },
    { key: 'DateOfStart', label: 'Дата начала', type: 'date' },
    { key: 'DateOfEnd', label: 'Дата окончания (необяз.)', type: 'date', optional: true }
  ]
};

const TITLES = {
  CntrEmployers: 'Сотрудники',
  CntrProfessions: 'Профессии',
  CntrProfessionEmployer: 'Проф. назначения',
  CntrOperations: 'Операции',
  CntrWorks: 'Работы',
  CntrAccrualsType: 'Типы начислений',
  CntrAccruals: 'Начисления',
  CntrPayments: 'Выплаты'
};

// Столбцы для специального форматирования
const MONEY_COLS = ['AmountToPay', 'AccrualTotal', 'Bonus', 'RatePerUnit'];
const DATE_COLS = ['PaymentDate', 'WorkDate', 'DateOfStart', 'DateOfEnd'];
const ID_COLS = ['EmployeeID', 'OperationID', 'WorkID', 'AccrualsTypeID', 'ProfessionID', 'ProfessionEmployerID', 'AccrualID', 'PaymentID'];

// Авторизация
async function doLogin() {
  const email = document.getElementById('loginEmail').value.trim();
  const password = document.getElementById('loginPassword').value;
  const errEl = document.getElementById('loginErr');
  errEl.style.display = 'none';
  try {
    const res = await fetch(`${API}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });
    if (!res.ok) {
      errEl.style.display = 'block';
      return;
    }
    const data = await res.json();
    accessToken = data.accessToken;
    refreshToken = data.refreshToken;
    sessionStorage.setItem('accessToken', accessToken);
    sessionStorage.setItem('refreshToken', refreshToken);
    sessionStorage.setItem('userEmail', email);
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('appPage').style.display = 'flex';
    const firstItem = document.querySelector('.sidebar-item[data-ctrl]');
    if (firstItem) {
      firstItem.click();
    }
  } catch (e) {
    errEl.textContent = '⚠ Сервер недоступен';
    errEl.style.display = 'block';
  }
}

document.addEventListener('keydown', e => {
  if (e.key === 'Enter' && document.getElementById('loginPage').style.display !== 'none') {
    doLogin();
  }
});

function doLogout() {
  accessToken = '';
  refreshToken = '';
  sessionStorage.clear();
  document.getElementById('appPage').style.display = 'none';
  document.getElementById('loginPage').style.display = 'flex';
  document.getElementById('loginEmail').value = '';
  document.getElementById('loginPassword').value = '';
}

// API запросы
async function apiFetch(url, method = 'GET', body = null) {
  const opts = {
    method,
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${accessToken}`
    }
  };
  if (body !== null) opts.body = JSON.stringify(body);
  const res = await fetch(`${API}/${url}`, opts);
  if (!res.ok) {
    const msg = await res.text().catch(() => res.status);
    throw new Error(msg || res.status);
  }
  const text = await res.text();
  if (!text || text.trim() === '') return null;
  return JSON.parse(text);
}

// Работа с таблицами
async function loadTable(ctrl, sideEl) {
  currentCtrl = ctrl;

  document.querySelectorAll('.sidebar-item').forEach(el => el.classList.remove('active'));
  if (sideEl) sideEl.classList.add('active');

  document.getElementById('tableTitle').textContent = TITLES[ctrl] || ctrl;
  document.getElementById('tableWrap').innerHTML = '<div class="state-msg"><img src="stickers/loading.gif" alt="" class="sticker-icon" style="width:48px;height:48px;margin-bottom:8px"/>Загрузка...</div>';

  try {
    const data = await apiFetch(ctrl);
    currentData = data || [];
    renderTable(currentData);
    document.getElementById('tableCount').textContent = `${currentData.length} записей`;
  } catch (e) {
    document.getElementById('tableWrap').innerHTML = `<div class="state-msg"><img src="stickers/error.png" alt="" class="sticker-icon" style="width:48px;height:48px;margin-bottom:8px"/>Ошибка: ${e.message}</div>`;
  }
}

function refreshTable() {
  if (currentCtrl) loadTable(currentCtrl, null);
}

function renderTable(data) {
  const wrap = document.getElementById('tableWrap');
  if (!data || data.length === 0) {
    wrap.innerHTML = '<div class="state-msg"><img src="stickers/nodata.png" alt="" class="sticker-icon" style="width:48px;height:48px;margin-bottom:8px"/>Нет данных</div>';
    return;
  }
  const keys = Object.keys(data[0]);
  let html = '<table><thead><tr>';
  keys.forEach(k => {
    html += `<th>${k}</th>`;
  });
  html += '<th>Действия</th></tr></thead><tbody>';

  data.forEach(row => {
    html += '<tr>';
    keys.forEach(k => {
      let cls = '';
      let val = row[k] === null || row[k] === undefined ? '—' : row[k];
      if (MONEY_COLS.includes(k)) {
        cls = 'td-money';
        if (typeof val === 'number') val = val.toFixed(2) + ' ₽';
      } else if (DATE_COLS.includes(k)) {
        cls = 'td-date';
        if (val && val !== '—') val = formatDate(val);
      } else if (ID_COLS.includes(k) && keys.indexOf(k) === 0) {
        cls = 'td-id';
      } else if (k.toLowerCase().includes('name') || k === 'FullName' || k === 'Title' || k === 'Description') {
        cls = 'td-name';
      }
      html += `<td class="${cls}" title="${String(val).replace(/"/g, "'")}">${val}</td>`;
    });
    const idVal = row[keys[0]];
    html += `<td><div class="row-actions"><button class="btn-del" onclick="deleteRow('${currentCtrl}', ${idVal})">✕ Удалить</button></div></td>`;
    html += '</tr>';
  });
  html += '</tbody></table>';
  wrap.innerHTML = html;
}

function formatDate(val) {
  try {
    const d = new Date(val);
    if (isNaN(d)) return val;
    return d.toLocaleDateString('ru-RU');
  } catch {
    return val;
  }
}

async function deleteRow(ctrl, id) {
  if (!confirm('Удалить запись?')) return;
  try {
    await apiFetch(`${ctrl}?id=${id}`, 'DELETE');
    notify('Запись удалена', 'success');
    loadTable(ctrl, null);
  } catch (e) {
    notify('Ошибка: ' + e.message, 'error');
  }
}

// Модальное окно добавления
async function openAddModal() {
  if (!currentCtrl) {
    notify('Выберите таблицу', 'error');
    return;
  }
  const fields = FIELDS[currentCtrl];
  if (!fields) {
    notify('Форма для этой таблицы не настроена', 'error');
    return;
  }

  document.getElementById('modalTitle').textContent = 'Добавить — ' + (TITLES[currentCtrl] || currentCtrl);
  const fWrap = document.getElementById('modalFields');
  fWrap.innerHTML = '<div style="color:var(--text3);font-family:\'IBM Plex Mono\',monospace;font-size:12px;padding:8px 0">Загрузка формы...</div>';
  document.getElementById('addModal').classList.add('open');

  const selectFields = fields.filter(f => f.type === 'select');
  const selectData = {};
  await Promise.all(selectFields.map(async f => {
    try {
      selectData[f.key] = await apiFetch(f.source);
    } catch {
      selectData[f.key] = [];
    }
  }));

  fWrap.innerHTML = '';
  fields.forEach(f => {
    const div = document.createElement('div');
    div.className = 'modal-field';
    const optLabel = f.optional ? ' <span style="color:var(--text3)">(необяз.)</span>' : '';

    if (f.type === 'select') {
      const rows = selectData[f.key] || [];
      const opts = rows.length === 0
        ? '<option value="" disabled>— нет записей —</option>'
        : rows.map(r => `<option value="${r[f.valueKey]}">${f.labelFn(r)}</option>`).join('');
      div.innerHTML = `<label>${f.label}${optLabel}</label><select id="mf_${f.key}"><option value="">— выберите —</option>${opts}</select>`;
    } else {
      div.innerHTML = `<label>${f.label}${optLabel}</label>`
        + `<input type="${f.type || 'text'}" id="mf_${f.key}" placeholder="${f.placeholder || ''}" autocomplete="off"/>`;
    }
    fWrap.appendChild(div);
  });
  if (currentCtrl === 'CntrPayments'){
    const empSelect = document.getElementById('mf_EmployeeID');
    if (empSelect){
      empSelect.addEventListener('change', () => calcPaymentAmount(empSelect.value));
    }
  }
}

function closeModal() {
  document.getElementById('addModal').classList.remove('open');
  const bd = document.getElementById('payment-breakdown');
  if(bd)
    bd.remove();
}

async function submitAdd() {
  const fields = FIELDS[currentCtrl];
  const body = {};
  for (const f of fields) {
    const el = document.getElementById(`mf_${f.key}`);
    let val = el ? el.value.trim() : '';
    if (f.optional && val === '') continue;

    if (f.type === 'select') {
      if (!val) {
        notify(`Выберите «${f.label}»`, 'error');
        return;
      }
      body[f.key] = parseInt(val, 10);
    } else if (f.type === 'number') {
      if (val === '' && !f.optional) {
        notify(`Заполните поле «${f.label}»`, 'error');
        return;
      }
      body[f.key] = parseFloat(val) || 0;
    } else if (f.type === 'date') {
      if (val === '' && !f.optional) {
        notify(`Заполните поле «${f.label}»`, 'error');
        return;
      }
      body[f.key] = val ? new Date(val).toISOString() : null;
    } else {
      if (!val && !f.optional) {
        notify(`Заполните поле «${f.label}»`, 'error');
        return;
      }
      body[f.key] = val;
    }
  }
  try {
    await apiFetch(currentCtrl, 'POST', body);
    closeModal();
    notify('Запись добавлена', 'success');
    loadTable(currentCtrl, null);
  } catch (e) {
    notify('Ошибка: ' + e.message, 'error');
  }
}

async function calcPaymentAmount(employeeID) {
  if (!employeeID)
    return;
  const amountInput = document.getElementById('mf_AmountToPay');
  if (!amountInput)
    return;
  amountInput.value = '';
  amountInput.placeholder = 'Производится расчет...';

  try{
    const [allWorks, allAccruals, allAccrualsTypes] = await Promise.all([
      apiFetch('CntrWorks'),
      apiFetch('CntrAccruals'),
      apiFetch('CntrAccrualsType')
    ]);
    const empId = parseInt(employeeID, 10);
    const empWorks = (allWorks || []).filter(w => w.employeeID === empId);
    const empWorkIds = new Set(empWorks.map(w => w.workID));
    if (empWorkIds.size === 0){
      amountInput.placeholder = 'Нет работ, введите вручную';
      return;
    }
    const typeMap = {};
  (allAccrualsTypes || []).forEach(t => {
    typeMap[t.accrualsTypeID] = t.position;
   });
   let total = 0;
   const breakdown = [];
   empAccruals.forEach(a => {
    const position = typeMap[a.accrualsTypeID] ?? 1;
    const sum = (a.accrualTotal || 0) + (a.bonus || 0);
    total += position * sum;
    breakdown.push({position, sum, total: position * sum});
   });
   total = Math.max(0, total);

   amountInput.value = total.toFixed(2);
   amountInput.placeholder = '0';
   showPaymentBreakdown(empAccruals, allAccrualsTypes, typeMap)
  }
  catch(e){
    amountInput.placeholder = 'Ошибка расчета'
  }
}

function showPaymentBreakdown(accruals, types, typeMap){
  const old = document.getElementById('payment-breakdown');
  if (old) old.remove();
  const typeNames = {};
  (types || []).forEach(t => {typeNames[t.accrualsTypeID] = t.name; });
  const breakdown = document.createElement('div');
  breakdown.id = 'payment-breakdown';
  breakdown.style.cssText = `
  margin-top: 8px;
  background: var(--bg3);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 10px 12px;
  font-family: 'IBM Plex Mono', monospace;
  font-size: 11px;
  `;

  let html = '<div style="color:var(--text3);margin-bottom:6px;letter-spacing:0.5px">Расшифровка Начислений</div>';

  let total = 0;
  accruals.forEach(a => {
    const pos = typeMap[a.accrualsTypeID] ?? 1;
    const sum = (a.accrualTotal || 0) + (a.bonus || 0);
    const delta = pos * sum;
    total += delta;
    const color = pos >= 0 ? 'var(--accent2)' : 'var(--accent3)';
    const sign = pos >= 0 ? '+' : '-';
    const name = typeNames[a.accrualsTypeID] || `Тип #${a.accrualsTypeID}`;
    html += `<div style="display:flex;justify-content:space-between;padding:2px 0;border-bottom:1px solid rgba(48,54,61,0.5)">
    <span style="color:var(--text2)">${name}</span>
    <span style="color:${color}">${sign} ${sum.toFixed(2)} ₽</span>
    </div>`;
    breakdown.innerHTML = html;
    const amountField = document.getElementById('mf_AmountToPay');
    if (amountField)
       amountField.closest('.modal-field').after(breakdown);
    })
};

async function submitAdd() {
  const fields = FIELDS[currentCtrl];
  const body = {};
  for (const f of fields) {
    const el = document.getElementById(`mf_${f.key}`);
    let val = el ? el.value.trim() : '';
    if (f.optional && val === '')
       continue;

    if (f.type === 'select') {
      if (!val) { notify(`Выберите «${f.label}»`, 'error'); return; }
      body[f.key] = parseInt(val, 10);
    } 
    else if (f.type === 'number') {
      if (val === '' && !f.optional) { notify(`Заполните поле «${f.label}»`, 'error'); return; }
      body[f.key] = parseFloat(val) || 0;
    } 
    else if (f.type === 'date') {
      if (val === '' && !f.optional) { notify(`Заполните поле «${f.label}»`, 'error'); return; }
      body[f.key] = val ? new Date(val).toISOString() : null;
    } 
    else {
      if (!val && !f.optional) { notify(`Заполните поле «${f.label}»`, 'error'); return; }
      body[f.key] = val;
    }
  }
  try {
    await apiFetch(currentCtrl, 'POST', body);
    closeModal();
    notify('Запись добавлена', 'success');
    loadTable(currentCtrl, null);
  }
  catch(e) {
    notify('Ошибка: ' + e.message, 'error');
  }
}

// Переключение вкладок
function switchTab(tab, btn) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  btn.classList.add('active');
}

// Уведомления
let notifTimer = null;
function notify(msg, type = 'success') {
  const el = document.getElementById('notif');
  el.textContent = msg;
  el.className = type;
  el.style.display = 'block';
  clearTimeout(notifTimer);
  notifTimer = setTimeout(() => {
    el.style.display = 'none';
  }, 3000);
}

// Закрытие модалки по клику на оверлей
document.getElementById('addModal').addEventListener('click', function(e) {
  if (e.target === this) closeModal();
});

// Восстановление сессии
(function restoreSession() {
  const savedAccess = sessionStorage.getItem('accessToken');
  const savedRefresh = sessionStorage.getItem('refreshToken');
  const savedEmail = sessionStorage.getItem('userEmail');
  if (savedAccess) {
    accessToken = savedAccess;
    refreshToken = savedRefresh || '';
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('appPage').style.display = 'flex';
    const firstItem = document.querySelector('.sidebar-item[data-ctrl]');
    if (firstItem) {
      firstItem.click();
    }
  }
});