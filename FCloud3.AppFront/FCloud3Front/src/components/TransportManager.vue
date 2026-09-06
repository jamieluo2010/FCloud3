<template>
  <div class="transport-root">
    <h2>交通</h2>

    <div class="uploader">
      <label :for="`transportFile_${randId}`" class="uploadBtn" :class="{disabled:!canEdit}">选择 HTML 文件</label>
      <input :id="`transportFile_${randId}`" type="file" accept="text/html" @change="onFileChange" />

      <input v-model="uploadName" placeholder="显示名称" :disabled="!canEdit" />
      <input type="color" v-model="uploadColor" :disabled="!canEdit" />
      <button @click="upload" :disabled="!canEdit || !selectedFile">上传</button>
    </div>

    <div class="list">
      <div v-if="items.length===0" class="empty">尚无交通项，登录后可上传自己的 HTML 展示。</div>
      <div v-for="it in items" :key="it.Id" class="transport-item">
        <div class="left">
          <div class="name" :style="{color: it.Color || '#000'}">{{ it.Name }}</div>
          <div class="meta">上传者: 你 &nbsp;·&nbsp; {{ formatDate(it.CreatedAt) }}</div>
        </div>
        <div class="right">
          <a v-if="it.FileUrl" :href="it.FileUrl" target="_blank">查看</a>
          <button @click="startEdit(it)" :disabled="!canEdit">编辑</button>
          <button @click="remove(it.Id)" :disabled="!canEdit">删除</button>
        </div>
      </div>
    </div>

    <div v-if="editing" class="editPanel">
      <h3>编辑</h3>
      <input v-model="editingName" placeholder="显示名称" />
      <input type="color" v-model="editingColor" />
      <button @click="saveEdit">保存</button>
      <button @click="cancelEdit">取消</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useIdentityInfoStore } from '@/utils/globalStores/identityInfo';
import { injectApi } from '@/provides';

interface TransportItem {
  Id: number;
  Name: string;
  Color?: string;
  FileUrl?: string;
  CreatedAt?: string;
}

const api = injectApi();
const iden = useIdentityInfoStore();
const isLoginValid = computed(() => iden.isLoginValid);
const canEdit = computed(() => isLoginValid.value);

const items = ref<TransportItem[]>([]);
const selectedFile = ref<File | null>(null);
const uploadName = ref('');
const uploadColor = ref('#2196f3');
const randId = Math.floor(Math.random()*1000000);

const editing = ref(false);
const editingId = ref<number | null>(null);
const editingName = ref('');
const editingColor = ref('#2196f3');

function formatDate(s?:string){
  if(!s) return '';
  try{ return new Date(s).toLocaleString(); }catch{ return s; }
}

function onFileChange(e:Event){
  const input = e.target as HTMLInputElement;
  if(input.files && input.files.length>0){
    selectedFile.value = input.files[0];
    if(!uploadName.value) uploadName.value = selectedFile.value.name.replace(/\.html?$/i, '');
  }
}

async function load(){
  try{
    const res = await api.transport.getList();
    if(res) items.value = res as TransportItem[];
  }catch{}
}

async function upload(){
  if(!canEdit.value) return;
  if(!selectedFile.value) return;
  const payload = { file: selectedFile.value, Name: uploadName.value || selectedFile.value.name, Color: uploadColor.value };
  const resp = await api.transport.upload(payload);
  if(resp){
    selectedFile.value = null;
    uploadName.value = '';
    await load();
  }
}

function startEdit(it:TransportItem){
  editing.value = true;
  editingId.value = it.Id;
  editingName.value = it.Name;
  editingColor.value = it.Color || '#2196f3';
}
function cancelEdit(){ editing.value = false; editingId.value = null; }

async function saveEdit(){
  if(!editingId.value) return;
  const payload = { Id: editingId.value, Name: editingName.value, Color: editingColor.value };
  const resp = await api.transport.edit(payload);
  if(resp){ await load(); cancelEdit(); }
}

async function remove(id?:number){
  if(!id || !canEdit.value) return;
  const resp = await api.transport.remove(id);
  if(resp) await load();
}

// initial load
load();
</script>

<style scoped lang="scss">
.transport-root{ border-top:1px solid #eee; padding-top:18px; margin-top:18px; }
.uploader{ display:flex; gap:8px; align-items:center; flex-wrap:wrap; margin-bottom:12px; }
.uploadBtn{ background:#eee; padding:6px 10px; border-radius:6px; cursor:pointer; }
.uploadBtn.disabled{ opacity:0.5; pointer-events:none; }
.list{ display:flex; flex-direction:column; gap:8px; }
.transport-item{ display:flex; justify-content:space-between; padding:8px; border-bottom:1px dashed #eee; }
.name{ font-weight:600; }
.meta{ font-size:12px; color:#888; }
.editPanel{ margin-top:10px; border:1px solid #ddd; padding:10px; border-radius:6px; }
</style>
