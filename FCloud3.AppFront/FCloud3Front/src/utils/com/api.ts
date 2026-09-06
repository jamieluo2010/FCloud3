@@
 export class Api{
     private httpClient: HttpClient;
     constructor(httpClient:HttpClient){
         this.httpClient = httpClient;
     }
+    transport = {
+        getList: async() => {
+            const res = await this.httpClient.request('/api/Transport/GetList', 'get');
+            if(res.success) return res.data;
+        },
+        upload: async(payload:{file:File, Name:string, Color?:string}) => {
+            const res = await this.httpClient.request('/api/Transport/Upload', 'postForm', payload, '上传成功', true);
+            return res.success;
+        },
+        edit: async(payload:{Id:number, Name:string, Color?:string}) => {
+            const res = await this.httpClient.request('/api/Transport/Edit', 'postRaw', payload, '保存成功', true);
+            return res.success;
+        },
+        remove: async(id:number) => {
+            const res = await this.httpClient.request('/api/Transport/Remove', 'get', { id }, '删除成功', true);
+            return res.success;
+        }
+    }
@@
 }
