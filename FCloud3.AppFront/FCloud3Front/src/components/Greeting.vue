<template>
  <div class="greeting-root" aria-live="polite">
    <h1 class="greeting-title">{{ text }}</h1>

    <!-- 深夜提醒：只在深夜显示，并可关闭（按天记录到 localStorage） -->
    <div v-if="isLate && showReminder" class="late-reminder" role="status">
      <span>夜深了，注意休息</span>
      <button class="dismiss-btn" @click="dismiss">知道了</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

const text = ref('欢迎');
const isLate = ref(false);
const showReminder = ref(false);
const REMINDER_KEY = 'greeting_late_dismissed_on'; // 存储关闭状态（按天）

function computeGreeting(hour: number) {
  if (hour >= 5 && hour < 11) return { text: '早安', late: false };
  if (hour >= 11 && hour < 18) return { text: '午安', late: false };
  if (hour >= 18 && hour < 23) return { text: '晚安', late: false };
  return { text: '晚安', late: true }; // 23:00 - 04:59 当作深夜
}

function isDismissedToday(): boolean {
  try {
    const v = localStorage.getItem(REMINDER_KEY);
    if (!v) return false;
    return v === new Date().toISOString().slice(0,10); // YYYY-MM-DD
  } catch {
    return false;
  }
}

function dismiss() {
  try {
    localStorage.setItem(REMINDER_KEY, new Date().toISOString().slice(0,10));
  } catch {}
  showReminder.value = false;
}

onMounted(() => {
  const now = new Date();
  const hour = now.getHours();
  const g = computeGreeting(hour);
  text.value = g.text;
  isLate.value = g.late;

  if (g.late && !isDismissedToday()) {
    showReminder.value = true;
  }
});
</script>

<style scoped lang="scss">
.greeting-root { display: flex; flex-direction: column; gap: 6px; align-items: flex-start; }
.greeting-title { font-size: 1.6rem; margin: 0; }
.late-reminder {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: #fff4f4;
  color: #b00020;
  border: 1px solid #f5c6cb;
  padding: 6px 10px;
  border-radius: 6px;
  font-size: 0.95rem;
}
.dismiss-btn {
  background: transparent;
  border: none;
  color: #b00020;
  cursor: pointer;
  padding: 4px 6px;
  font-size: 0.9rem;
}
.dismiss-btn:focus { outline: 2px solid rgba(176,0,32,0.25); border-radius: 4px; }
</style>
