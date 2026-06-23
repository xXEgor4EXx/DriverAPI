// Конфиг
const API = 'http://localhost:5122';

// Переменные состояния
let accessToken = '';
let refreshToken = '';
let currentCtrl = '';
let currentData = [];
let editingRowId = null; // Для редактирования

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
      labelFn: r => `${r.fullName}`
    },
    {
      key: 'OperationID', label: 'Операция', type: 'select',
      source: 'CntrOperations', valueKey: 'operationID',
      labelFn: r => ` ${r.description} — ${r.ratePerUnit} ₽/ед.`
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
      labelFn: r => `${r.name}`
    },
    {
      key: 'WorkID', label: 'Наряд / Работа', type: 'select',
      source: 'CntrWorks', valueKey: 'workID',
      labelFn: (r) => {
        const empRows = _refCache['CntrEmployers'] || [];
        const opRows = _refCache['CntrOperations'] || [];
        
        const emp = empRows.find(e => e.employeeID == r.employeeID);
        const op = opRows.find(o => o.operationID == r.operationID);
        
        const empName = emp ? emp.fullName : (`Сотр.#${r.employeeID}`);
        const opName = op ? op.description : '';
        const date = r.workDate ? new Date(r.workDate).toLocaleDateString('ru-RU') : '—';
        return `${empName} — ${date}${opName ? ` (${opName})` : ''}`;
      }
    },
    { key: 'Bonus', label: 'Премия (₽)', type: 'number', placeholder: '0' },
    { key: 'AccrualTotal', label: 'Сумма начисления (₽)', type: 'number', placeholder: '0' }
  ],
  CntrPayments: [
    {
      key: 'EmployeeID', label: 'Сотрудник', type: 'select',
      source: 'CntrEmployers', valueKey: 'employeeID',
      labelFn: r => ` ${r.fullName}`
    },
    { key: 'AmountToPay', label: 'Сумма к выдаче (₽)', type: 'number', placeholder: '0' },
    { key: 'PaymentDate', label: 'Дата выплаты', type: 'date' }
  ],
  CntrProfessionEmployer: [
    {
      key: 'EmployeeID', label: 'Сотрудник', type: 'select',
      source: 'CntrEmployers', valueKey: 'employeeID',
      labelFn: r => ` ${r.fullName}`
    },
    {
      key: 'ProfessionID', label: 'Профессия', type: 'select',
      source: 'CntrProfessions', valueKey: 'professionID',
      labelFn: r => ` ${r.title}`
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

// Человекочитаемые заголовки колонок
const COL_LABELS = {
  fullName:             'ФИО',
  phone:                'Телефон',
  title:                'Название профессии',
  description:          'Описание операции',
  ratePerUnit:          'Расценка (руб./ед.)',
  workDate:             'Дата работы',
  quantity:             'Выполнено (шт.)',
  rejectedQuantity:     'Брак (шт.)',
  name:                 'Описание',
  position:             'Позиция',
  bonus:                'Премия (руб.)',
  accrualTotal:         'Сумма начисления (руб.)',
  amountToPay:          'Сумма к выдаче (руб.)',
  paymentDate:          'Дата выплаты',
  dateOfStart:          'Дата начала',
  dateOfEnd:            'Дата окончания',
};

// FK-поля: скрываем числовой ID, показываем читаемое значение
const FK_HIDDEN = new Set(['employeeID', 'operationID', 'workID', 'accrualsTypeID', 'professionID']);

// Кеш справочников
let _refCache = {};

async function loadRefCache(ctrl) {
  const needed = new Set();
  (FIELDS[ctrl] || []).forEach(f => {
    if (f.type === 'select') needed.add(f.source);
  });
  if (ctrl === 'CntrAccruals') {
    needed.add('CntrEmployers');
    needed.add('CntrOperations');
  }
  await Promise.all([...needed].map(async src => {
    if (!_refCache[src]) {
      try { _refCache[src] = await apiFetch(src); }
      catch { _refCache[src] = []; }
    }
  }));
}

// Читаемый лейбл записи для ячейки таблицы
function cleanLabel(src, row) {
  if (src === 'CntrEmployers')    return row.fullName;
  if (src === 'CntrOperations')   return `${row.description} (${row.ratePerUnit} руб./ед.)`;
  if (src === 'CntrProfessions')  return row.title;
  if (src === 'CntrAccrualsType') return row.name;
  if (src === 'CntrWorks') {
    const empRows = _refCache['CntrEmployers'] || [];
    const opRows  = _refCache['CntrOperations'] || [];
    const emp = empRows.find(e => e.employeeID == row.employeeID);
    const op  = opRows.find(o => o.operationID == row.operationID);
    const empName = emp ? emp.fullName : (`Сотр.#${row.employeeID}`);
    const opName  = op ? op.description : '';
    const date = row.workDate ? new Date(row.workDate).toLocaleDateString('ru-RU') : '—';
    return `${empName} — ${date}${opName ? ` (${opName})` : ''}`;
  }
  return row.fullName || row.title || row.name || ('#' + row[Object.keys(row)[0]]);
}

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
      const text = await res.text();
      let msg = 'Неверный логин или пароль.';
      try {
        const j = JSON.parse(text);
        if (j.error) msg = j.error;
        else if (j.message) msg = j.message;
      } catch {}
      errEl.textContent = '⚠ ' + msg;
      errEl.style.display = 'block';
      return;
    }
    const data = await res.json();
    accessToken = data.accessToken;
    refreshToken = data.refreshToken;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('userEmail', email);
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('appPage').style.display = 'flex';
    const firstItem = document.querySelector('.sidebar-item[data-ctrl]');
    if (firstItem) {
      firstItem.click();
    }
  } catch (e) {
    errEl.textContent = 'Сервер недоступен';
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
  localStorage.clear();
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

  const fullUrl = `${API}/${url}`;
  console.log('API Request:', { url: fullUrl, method, body });

  const res = await fetch(fullUrl, opts);

  console.log('API Response:', { status: res.status, ok: res.ok });

  if (!res.ok) {
    const text = await res.text();
    console.error('API Error Response:', text);

    // Пробуем распарсить JSON-ошибку с бэкенда { error, detail }
    let humanMessage = null;
    try {
      const errJson = JSON.parse(text);
      if (errJson.error) {
        humanMessage = errJson.error;
      } else if (errJson.message) {
        humanMessage = errJson.message;
      } else if (errJson.errors) {
        const msgs = Object.values(errJson.errors).flat();
        humanMessage = msgs.join(' ');
      } else if (errJson.title) {
        humanMessage = errJson.title;
      }
    } catch {
    }

    if (!humanMessage) {
      humanMessage = HTTP_ERROR_MESSAGES[res.status] || `Ошибка сервера (код ${res.status})`;
    }

    throw new Error(humanMessage);
  }

  const text = await res.text();
  if (!text || text.trim() === '') return null;
  return JSON.parse(text);
}

const HTTP_ERROR_MESSAGES = {
  400: 'Некорректные данные запроса.',
  401: 'Требуется авторизация. Войдите заново.',
  403: 'Доступ запрещён.',
  404: 'Запись не найдена.',
  409: 'Конфликт: данные уже существуют или связаны с другими записями.',
  422: 'Ошибка валидации данных.',
  500: 'Внутренняя ошибка сервера. Попробуйте позже.',
  503: 'Сервер временно недоступен.'
};

// Работа с таблицами
async function loadTable(ctrl, sideEl) {
  currentCtrl = ctrl;

  document.querySelectorAll('.sidebar-item').forEach(el => el.classList.remove('active'));
  if (sideEl) sideEl.classList.add('active');

  document.getElementById('tableTitle').textContent = TITLES[ctrl] || ctrl;
  document.getElementById('tableWrap').innerHTML = '<div class="state-msg"><img src="stickers/loading.gif" alt="" class="sticker-icon" style="width:48px;height:48px;margin-bottom:8px"/>Загрузка...</div>';

  try {
    await loadRefCache(ctrl);
    const data = await apiFetch(ctrl);
    currentData = data || [];
    renderTable(currentData);
    document.getElementById('tableCount').textContent = `${currentData.length} записей`;
  } catch (e) {
    document.getElementById('tableWrap').innerHTML = `<div class="state-msg"><img src="stickers/empty.png" alt="" class="sticker-icon" style="width:48px;height:48px;margin-bottom:8px"/>${e.message}</div>`;
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

  const allKeys = Object.keys(data[0]);
  const pkKey = allKeys[0];

  // Строим список видимых колонок: FK-поля заменяем resolved-версией
  const visibleKeys = [];
  allKeys.forEach(k => {
    const kLow = k.charAt(0).toLowerCase() + k.slice(1);
    if (k === pkKey) return;
    const isFk = FK_HIDDEN.has(kLow);
    if (isFk) {
      const fieldDef = (FIELDS[currentCtrl] || []).find(f =>
        (f.key.charAt(0).toLowerCase() + f.key.slice(1)) === kLow
      );
      if (fieldDef) {
        visibleKeys.push({ key: k, label: fieldDef.label, isFk: true, fieldDef });
        return;
      }
    }
    visibleKeys.push({ key: k, label: COL_LABELS[kLow] || k, isFk: false });
  });

  // Заголовки
  let html = '<table><thead><tr>';
  visibleKeys.forEach(col => { html += '<th>' + col.label + '</th>'; });
  html += '<th>Действия</th></tr></thead><tbody>';

  // Строки
  data.forEach(row => {
    html += '<tr>';
    visibleKeys.forEach(col => {
      let cls = '', val;

      if (col.isFk) {
        const rawId = row[col.key];
        const refs = _refCache[col.fieldDef.source] || [];
        const found = refs.find(r => r[col.fieldDef.valueKey] == rawId);
        val = found ? cleanLabel(col.fieldDef.source, found) : (rawId != null ? '#' + rawId : '—');
        cls = 'td-name';
      } else {
        const k = col.key;
        val = row[k] === null || row[k] === undefined ? '—' : row[k];
        const kNorm = k.charAt(0).toUpperCase() + k.slice(1);
        if (MONEY_COLS.includes(kNorm)) {
          cls = 'td-money';
          if (typeof val === 'number')
            val = val.toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' ₽';
        } else if (DATE_COLS.includes(kNorm)) {
          cls = 'td-date';
          if (val && val !== '—') val = formatDate(val);
        } else if (k === pkKey) {
          cls = 'td-id';
        } else {
          const kLow = k.charAt(0).toLowerCase() + k.slice(1);
          if (['fullName','title','description','name'].includes(kLow)) cls = 'td-name';
        }
      }
      html += '<td class="' + cls + '" title="' + String(val).replace(/"/g, "'") + '">' + val + '</td>';
    });

    const idVal = row[pkKey];
    html += '<td><div class="row-actions">'
      + '<button class="btn-edit" onclick="editRow(\'' + currentCtrl + '\', ' + idVal + ')">Изменить</button>'
      + '<button class="btn-del" onclick="deleteRow(\'' + currentCtrl + '\', ' + idVal + ')">✕ Удалить</button>'
      + '</div></td></tr>';
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
  const row = currentData.find(r => r[Object.keys(r)[0]] == id);
  let label = `запись #${id}`;
  if (row) {
    const nameKey = Object.keys(row).find(k =>
      ['fullName', 'title', 'description', 'name'].includes(k.toLowerCase()) ||
      k.toLowerCase().includes('name') || k.toLowerCase().includes('title')
    );
    if (nameKey && row[nameKey]) label = `«${row[nameKey]}»`;
  }

  if (!confirm(`Удалить ${label}?`)) return;
  try {
    await apiFetch(`${ctrl}?id=${id}`, 'DELETE');
    notify('Запись удалена', 'success');
    loadTable(ctrl, null);
  } catch (e) {
    notify(e.message, 'error');
  }
}

function editRow(ctrl, id) {
  console.log('editRow called:', ctrl, id, 'currentData length:', currentData.length);
  
  if (!currentData || currentData.length === 0) {
    notify('Нет данных для редактирования', 'error');
    return;
  }
  const firstKey = Object.keys(currentData[0])[0];
  const rowData = currentData.find(row => {
    const rowId = row[firstKey];
    console.log('Comparing:', rowId, '===', id, '?', rowId === id);
    return rowId == id; 
  });
  
  if (rowData) {
    console.log('Found rowData:', rowData);
    openEditModal(ctrl, rowData);
  } else {
    notify('Запись не найдена', 'error');
    console.log('Row not found with id:', id);
    console.log('Available IDs:', currentData.map(r => r[firstKey]));
  }
}

// Модальное окно добавления
async function openAddModal() {
  if (!currentCtrl) {
    notify('Выберите таблицу', 'error');
    return;
  }
  editingRowId = null;
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
  if (currentCtrl === 'CntrAccruals') {
    const workSelect = document.getElementById('mf_WorkID');
    const typeSelect = document.getElementById('mf_AccrualsTypeID');
  
    const recalc = () => {
      if (workSelect.value && typeSelect.value) {
        calcAccrualTotal(workSelect.value, typeSelect.value);
      }
    };
  
    if (workSelect) workSelect.addEventListener('change', recalc);
    if (typeSelect) typeSelect.addEventListener('change', recalc);
  }
}

async function openEditModal(ctrl, rowData) {
  currentCtrl = ctrl;
  editingRowId = rowData[Object.keys(rowData)[0]];
  
  console.log(' Открытие редактирования:', { ctrl, editingRowId, rowData });
  
  const fields = FIELDS[ctrl];
  if (!fields) {
    notify('Форма для этой таблицы не настроена', 'error');
    return;
  }

  document.getElementById('modalTitle').textContent = 'Изменить — ' + (TITLES[ctrl] || ctrl);
  const fWrap = document.getElementById('modalFields');
  fWrap.innerHTML = '<div style="color:var(--text3);font-family:\'IBM Plex Mono\',monospace;font-size:12px;padding:8px 0">Загрузка формы...</div>';
  document.getElementById('addModal').classList.add('open');

  // Загружаем данные для select полей
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
    
    // Ключ в данных API (с маленькой буквы)
    const keyInData = f.key.charAt(0).toLowerCase() + f.key.slice(1);
    const currentValue = rowData[keyInData] !== undefined && rowData[keyInData] !== null ? rowData[keyInData] : '';
    
    console.log(` Поле ${f.key}: значение из данных =`, currentValue);
    
    if (f.type === 'select') {
      const rows = selectData[f.key] || [];
      const selectedId = rowData[keyInData] !== undefined ? rowData[keyInData] : '';
      
      // Находим выбранную запись для отображения понятного названия
      const selectedRow = rows.find(r => r[f.valueKey] == selectedId);
      const selectedLabel = selectedRow ? f.labelFn(selectedRow) : '— не выбрано —';
      
      console.log(`  → Select ${f.key}: ID=${selectedId}, название=${selectedLabel}`);
      
      const opts = rows.map(r => {
        const isSelected = r[f.valueKey] == selectedId;
        return `<option value="${r[f.valueKey]}" ${isSelected ? 'selected' : ''}>${f.labelFn(r)}</option>`;
      }).join('');
          
      div.innerHTML = `
        <label>${f.label}${optLabel}</label>
        <select id="mf_${f.key}">
          <option value="">— выберите —</option>
          ${opts}
        </select>
        ${selectedId ? `<div style="color:var(--text3);font-size:11px;margin-top:4px;">Текущее: ${selectedLabel}</div>` : ''}
      `;
    } else if (f.type === 'date') {
      let dateVal = '';
      let displayDate = '—';
      if (currentValue) {
        const d = new Date(currentValue);
        if (!isNaN(d)) {
          dateVal = d.toISOString().split('T')[0];
          displayDate = d.toLocaleDateString('ru-RU');
        }
      }
      div.innerHTML = `
        <label>${f.label}${optLabel}</label>
        <input type="date" id="mf_${f.key}" value="${dateVal}" autocomplete="off"/>
        ${currentValue ? `<div style="color:var(--text3);font-size:11px;margin-top:4px;">Текущее: ${displayDate}</div>` : ''}
      `;
    } else if (f.type === 'number') {
      let displayValue = currentValue;
      if (typeof currentValue === 'number') {
        displayValue = currentValue.toFixed(2);
      }
      div.innerHTML = `
        <label>${f.label}${optLabel}</label>
        <input type="number" step="0.01" id="mf_${f.key}" placeholder="${f.placeholder || ''}" value="${String(displayValue).replace(/"/g, '&quot;')}" autocomplete="off"/>
        ${currentValue !== '' && currentValue !== undefined ? `<div style="color:var(--text3);font-size:11px;margin-top:4px;">Текущее: ${displayValue}</div>` : ''}
      `;
    } else {
      div.innerHTML = `
        <label>${f.label}${optLabel}</label>
        <input type="${f.type || 'text'}" id="mf_${f.key}" placeholder="${f.placeholder || ''}" value="${String(currentValue).replace(/"/g, '&quot;')}" autocomplete="off"/>
        ${currentValue ? `<div style="color:var(--text3);font-size:11px;margin-top:4px;">Текущее: ${currentValue}</div>` : ''}
      `;
    }
    fWrap.appendChild(div);
  });
  
  // Добавляем режим редактирования
  const hint = document.createElement('div');
  hint.style.cssText = 'margin-top:12px;padding:8px 12px;background:var(--bg3);border-radius:6px;color:var(--text3);font-size:12px;border-left:3px solid var(--accent);';
  hint.textContent = ' Режим редактирования: измените нужные поля и нажмите "Сохранить"';
  fWrap.appendChild(hint);
  
  // Кнопки уже должны быть в модалке
  // Проверяем, что кнопка сохранения вызывает submitAdd
  const saveBtn = document.querySelector('#addModal .modal-actions .btn-primary');
  if (saveBtn) {
    saveBtn.onclick = submitAdd;
  }
  
  // Подписка на события для расчетов
  if (ctrl === 'CntrPayments'){
    const empSelect = document.getElementById('mf_EmployeeID');
    if (empSelect){
      empSelect.addEventListener('change', () => calcPaymentAmount(empSelect.value));
    }
  }
  if (currentCtrl === 'CntrAccruals') {
    const workSelect = document.getElementById('mf_WorkID');
    const typeSelect = document.getElementById('mf_AccrualsTypeID');
    
    const recalc = () => {
      if (workSelect.value && typeSelect.value) {
        calcAccrualTotal(workSelect.value, typeSelect.value);
      }
    };
    
    if (workSelect) workSelect.addEventListener('change', recalc);
    if (typeSelect) typeSelect.addEventListener('change', recalc);
  }
}

function closeModal() {
  document.getElementById('addModal').classList.remove('open');
  const bd = document.getElementById('payment-breakdown');
  if(bd) bd.remove();
  editingRowId = null;
}

async function submitAdd() {
  const fields = FIELDS[currentCtrl];
  const body = {};
  
  console.log(' Начинаем сохранение. Режим:', editingRowId !== null ? 'РЕДАКТИРОВАНИЕ' : 'ДОБАВЛЕНИЕ');
  console.log(' Текущие данные формы:');
  
  // Собираем данные из формы
  for (const f of fields) {
    const el = document.getElementById(`mf_${f.key}`);
    if (!el) {
      console.warn(` Элемент mf_${f.key} не найден`);
      continue;
    }
    
    let val = el.value.trim();
    const keyInData = f.key.charAt(0).toLowerCase() + f.key.slice(1);
    
    console.log(`  ${f.key} (${keyInData}) = "${val}" (тип: ${f.type})`);
    
    // Пропускаем пустые необязательные поля
    if (f.optional && val === '') continue;
    
    if (f.type === 'select') {
      if (!val) {
        notify(` Пожалуйста, выберите «${f.label}»`, 'error');
        return;
      }
      body[keyInData] = parseInt(val, 10);
    } else if (f.type === 'number') {
      if (val === '' && !f.optional) {
        notify(` Заполните поле «${f.label}»`, 'error');
        return;
      }
      // Очищаем строку от лишних символов (убираем пробелы, запятые)
      const cleanVal = val.replace(/\s/g, '').replace(',', '.');
      const numVal = parseFloat(cleanVal);
      if (isNaN(numVal) && val !== '') {
        notify(` Введите корректное число в поле «${f.label}»`, 'error');
        return;
      }
      body[keyInData] = numVal || 0;
    } else if (f.type === 'date') {
      if (val === '' && !f.optional) {
        notify(` Заполните поле «${f.label}»`, 'error');
        return;
      }
      body[keyInData] = val ? new Date(val).toISOString() : null;
    } else {
      if (!val && !f.optional) {
        notify(` Заполните поле «${f.label}»`, 'error');
        return;
      }
      body[keyInData] = val;
    }
  }
  
  console.log(' Сформирован body для отправки:', body);
  
  try {
    if (editingRowId !== null) {
      // РЕДАКТИРОВАНИЕ - отправляем PUT
      const firstKey = Object.keys(currentData[0] || {})[0];
      body[firstKey] = editingRowId;
      
      console.log(` PUT запрос на ${currentCtrl}?id=${editingRowId}`, body);
      const result = await apiFetch(`${currentCtrl}?id=${editingRowId}`, 'PUT', body);
      console.log(' Результат обновления:', result);
      notify(` Запись #${editingRowId} успешно обновлена`, 'success');
    } else {
      // ДОБАВЛЕНИЕ - отправляем POST
      console.log(`POST запрос на ${currentCtrl}`, body);
      const result = await apiFetch(currentCtrl, 'POST', body);
      console.log('Результат добавления:', result);
      notify(' Новая запись успешно добавлена', 'success');
    }
    
    closeModal();
    // Обновляем таблицу после сохранения
    await loadTable(currentCtrl, null);
  } catch (e) {
    console.error(' Ошибка при сохранении:', e);
    notify(` ${e.message}`, 'error');
  }
}

// Функция расчета суммы начисления
async function calcAccrualTotal(workID, accrualsTypeID) {
  if (!workID || !accrualsTypeID) return;
  
  const accrualTotalInput = document.getElementById('mf_AccrualTotal');
  if (!accrualTotalInput) return;
  
  accrualTotalInput.value = '';
  accrualTotalInput.placeholder = 'Расчет...';
  
  try {
    const [allWorks, allOperations, allAccrualsTypes] = await Promise.all([
      apiFetch('CntrWorks'),
      apiFetch('CntrOperations'),
      apiFetch('CntrAccrualsType')
    ]);
    
    const work = allWorks.find(w => w.workID == workID);
    if (!work) {
      accrualTotalInput.placeholder = 'Работа не найдена';
      return;
    }
    
    const operation = allOperations.find(op => op.operationID == work.operationID);
    if (!operation) {
      accrualTotalInput.placeholder = 'Операция не найдена';
      return;
    }
    
    const accrualType = allAccrualsTypes.find(t => t.accrualsTypeID == accrualsTypeID);
    if (!accrualType) {
      accrualTotalInput.placeholder = 'Тип не найден';
      return;
    }
    
    // Расчет оплаты за работу (годные детали)
    const acceptedQuantity = work.quantity - (work.rejectedQuantity || 0);
    const workPayment = acceptedQuantity * operation.ratePerUnit;
    
    let suggestedTotal = 0;

    if (accrualType.position === 1) {
      suggestedTotal = workPayment;
    } else if (accrualType.position === -1) {
      suggestedTotal = -workPayment * 0.13; 
    }
    
    // Округляем до 2 знаков
    accrualTotalInput.value = Math.abs(suggestedTotal).toFixed(2);
    accrualTotalInput.placeholder = '0';
    
  } catch (e) {
    console.error(e);
    accrualTotalInput.placeholder = 'Ошибка';
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
    const [allWorks, allOperations, allAccruals, allAccrualsTypes] = await Promise.all([
      apiFetch('CntrWorks'),
      apiFetch('CntrOperations'),
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
    
    // Создаём карту операций для быстрого доступа
    const operationMap = {};
    (allOperations || []).forEach(op => {
      operationMap[op.operationID] = op;
    });
    
    const typeMap = {};
    (allAccrualsTypes || []).forEach(t => {
      typeMap[t.accrualsTypeID] = t.position;
    });
    
    // Рассчитываем выплату за выполненные работы (с учётом брака)
    let workPayment = 0;
    const workBreakdown = [];
    empWorks.forEach(work => {
      const operation = operationMap[work.operationID];
      if (!operation) return;
      
      const acceptedQuantity = work.quantity - work.rejectedQuantity;
      const payment = acceptedQuantity * operation.ratePerUnit;
      workPayment += payment;
      workBreakdown.push({
        work,
        operation,
        acceptedQuantity,
        payment
      });
    });
    
    // Получаем все начисления (бонусы, налоги и т.д.)
    const empAccruals = (allAccruals || []).filter(a => empWorkIds.has(a.workID));
    
    let total = workPayment;
    empAccruals.forEach(a => {
      const position = typeMap[a.accrualsTypeID] ?? 1;
      const bonus = a.bonus || 0;
      const accrualSum = (a.accrualTotal || 0) + bonus;
      total += position * accrualSum;
    });
    
    total = Math.max(0, total);

    amountInput.value = total.toFixed(2);
    amountInput.placeholder = '0';
    showPaymentBreakdown(workBreakdown, empAccruals, allAccrualsTypes, typeMap, workPayment)
  }
  catch(e){
    console.error(e);
    amountInput.placeholder = 'Ошибка расчета'
  }
}

function showPaymentBreakdown(workBreakdown, accruals, types, typeMap, workPayment){
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
  max-height: 300px;
  overflow-y: auto;
  `;

  let html = '<div style="color:var(--text3);margin-bottom:6px;letter-spacing:0.5px;font-weight:bold">📋 Расшифровка Выплаты</div>';

  // Раздел работ
  html += '<div style="border-bottom:1px solid rgba(48,54,61,0.7);padding-bottom:6px;margin-bottom:6px">';
  html += '<div style="color:var(--text2);font-size:10px;margin-bottom:4px">Работы:</div>';
  workBreakdown.forEach(wb => {
    const rejected = wb.work.rejectedQuantity || 0;
    html += `<div style="display:flex;justify-content:space-between;padding:1px 0;font-size:10px">
    <span style="color:var(--text3)">${wb.work.quantity - rejected}/${wb.work.quantity} × ${wb.operation.ratePerUnit}₽</span>
    <span style="color:var(--accent2)">+${wb.payment.toFixed(2)}₽</span>
    </div>`;
    if (rejected > 0) {
      html += `<div style="display:flex;justify-content:space-between;padding:1px 0;font-size:9px;color:var(--accent3)">
      <span>   (брак: ${rejected})</span>
      </div>`;
    }
  });
  html += '</div>';

  // Раздел начислений/вычитаний
  let total = workPayment;
  accruals.forEach(a => {
    const pos = typeMap[a.accrualsTypeID] ?? 1;
    const bonus = a.bonus || 0;
    const accrualSum = (a.accrualTotal || 0) + bonus;
    const delta = pos * accrualSum;
    total += delta;
    const color = pos >= 0 ? 'var(--accent2)' : 'var(--accent3)';
    const sign = pos >= 0 ? '+' : '';
    const name = typeNames[a.accrualsTypeID] || `Тип #${a.accrualsTypeID}`;
    html += `<div style="display:flex;justify-content:space-between;padding:2px 0;border-bottom:1px solid rgba(48,54,61,0.5)">
    <span style="color:var(--text2)">${name}</span>
    <span style="color:${color}">${sign}${delta.toFixed(2)}₽</span>
    </div>`;
  });
  
  html += `<div style="display:flex;justify-content:space-between;padding:4px 0;margin-top:6px;font-weight:bold;color:var(--accent)">
  <span>К ВЫДАЧЕ</span>
  <span>${Math.max(0, total).toFixed(2)}₽</span>
  </div>`;
  
  breakdown.innerHTML = html;
  const amountField = document.getElementById('mf_AmountToPay');
  if (amountField)
    amountField.closest('.modal-field').after(breakdown);
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
  // Ошибки держим дольше — текст нужно успеть прочитать
  const duration = type === 'error' ? 6000 : 3000;
  notifTimer = setTimeout(() => {
    el.style.display = 'none';
  }, duration);
}

// Закрытие модалки по клику на оверлей
document.getElementById('addModal').addEventListener('click', function(e) {
  if (e.target === this) closeModal();
});

// Восстановление сессии
(function restoreSession() {
  const savedAccess = localStorage.getItem('accessToken');
  const savedRefresh = localStorage.getItem('refreshToken');
  const savedEmail = localStorage.getItem('userEmail');
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
})();