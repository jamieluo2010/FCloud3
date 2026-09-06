<template>
  <div class="currency-root">
    <h2>架空货币换算</h2>

    <div class="currency-controls">
      <div class="currency-list">
        <div v-for="c in currencies" :key="c.Id || c.code" class="currency-item">
          <div class="left">
            <strong>{{ c.Name }}</strong>
            <div class="sub">代码: {{ c.Code }}</div>
          </div>
          <div class="right">
            <input type="number" v-model.number="c.Rate" step="0.0001" :disabled="!canEdit" />
            <button @click="saveCurrency(c)" :disabled="!canEdit">保存</button>
            <button @click="removeCurrency(c.Id)" :disabled="!canEdit">删除</button>
          </div>
        </div>
      </div>

      <div class="adder">
        <input v-model="newName" placeholder="货币名称 (例如：云币)" />
        <input v-model="newCode" placeholder="三字码 (例如：YB)" />
        <input type="number" v-model.number="newRate" placeholder="基准汇率 (例如：0.5)" />
        <button @click="addCurrency" :disabled="!canEdit">添加货币</button>
      </div>

      <div class="converter">
        <input type="number" v-model.number="amount" />
        <select v-model="from">
          <option v-for="c in currencies" :key="c.Id" :value="c.Id">{{ c.Name }} ({{ c.Code }})</option>
        </select>
        <select v-model="to">
          <option v-for="c in currencies" :key="c.Id" :value="c.Id">{{ c.Name }} ({{ c.Code }})</option>
        </select>
        <button @click="convert">转换</button>
        <div v-if="result !== null" class="result">结果: {{ result }}</div>
      </div>
    </div>

    <div v-if="!isLoginValid" class="hint">注意：游客无法编辑货币类型，登录后可添加与管理自己的货币。</div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useIdentityInfoStore } from '@/utils/globalStores/identityInfo';
import { injectHttp } from '@/provides';

interface Currency {
  Id?: number;
  Name: string;
  Code: string;
  Rate: number; // 相对于基准货币（由用户定义）
}

const identity = useIdentityInfoStore();
const isLoginValid = computed(() => identity.isLoginValid);
const http = injectHttp();

const currencies = ref<Currency[]>([]);
const newName = ref('');
const newCode = ref('');
const newRate = ref<number>(1);
const amount = ref<number>(1);
const from = ref<number | null>(null);
const to = ref<number | null>(null);
const result = ref<number | null>(null);

function apiGetList() {
  return http.request('/api/FictionalCurrency/GetList', 'get');
}
function apiAdd(cur: Currency) {
  return http.request('/api/FictionalCurrency/Add', 'postRaw', cur, '添加成功', true);
}
function apiEdit(cur: Currency) {
  return http.request('/api/FictionalCurrency/Edit', 'postRaw', cur, '保存成功', true);
}
function apiRemove(id: number) {
  return http.request('/api/FictionalCurrency/Remove', 'get', { id }, '删除成功', true);
}

async function load() {
  try {
    if (!isLoginValid.value) {
      currencies.value = [];
      return;
    }
    const resp = await apiGetList();
    if (resp && resp.success) {
      currencies.value = resp.data as Currency[];
      if (currencies.value.length > 0) {
        from.value = currencies.value[0].Id || null;
        to.value = currencies.value.length > 1 ? currencies.value[1].Id || null : from.value;
      }
    }
  } catch (e) {
    currencies.value = [];
  }
}

async function addCurrency() {
  if (!isLoginValid.value) return;
  if (!newName.value || !newCode.value) return;
  const payload: Currency = { Name: newName.value, Code: newCode.value.toUpperCase(), Rate: newRate.value };
  const resp = await apiAdd(payload);
  if (resp && resp.success) {
    newName.value = '';
    newCode.value = '';
    newRate.value = 1;
    await load();
  }
}

async function removeCurrency(id?: number) {
  if (!isLoginValid.value || !id) return;
  const resp = await apiRemove(id);
  if (resp && resp.success) {
    await load();
  }
}

async function saveCurrency(c: Currency) {
  if (!isLoginValid.value) return;
  if (!c.Id) return;
  await apiEdit(c);
  await load();
}

function convert() {
  if (!from.value || !to.value) return;
  const f = currencies.value.find(c => c.Id === from.value);
  const t = currencies.value.find(c => c.Id === to.value);
  if (!f || !t) return;
  // 由用户定义的基准率，直接按比例换算：先将 amount 转为基准单位 (amount / f.Rate) 再乘以 t.Rate
  const baseAmount = amount.value / f.Rate;
  result.value = +(baseAmount * t.Rate).toFixed(4);
}

onMounted(async () => {
  await load();
});
</script>

<style scoped lang="scss">
.currency-root { border-top: 1px solid #eee; padding-top: 18px; margin-top: 18px; }
.currency-controls { display: flex; gap: 20px; flex-wrap: wrap; }
.currency-list { flex: 1 1 300px; max-width: 480px; }
.currency-item { display: flex; justify-content: space-between; padding: 8px; border-bottom: 1px dashed #eee; }
.adder { display: flex; gap: 8px; align-items: center; }
.converter { display: flex; gap: 8px; align-items: center; }
.result { margin-left: 8px; font-weight: bold; }
.hint { color: #888; margin-top: 8px; }
</style>
