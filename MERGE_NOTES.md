# 合并 feat/greeting-currency 到 master

## 合并信息

**来自分支**: `feat/greeting-currency`  
**目标分支**: `master`  
**合并时间**: 2026-09-13

## 主要变更概述

### 1. 后端新增功能 - 交通运输系统

#### 新文件
- `FCloud3.App/Controllers/Etc/TransportController.cs`
  - GetList() - 获取当前用户的交通项列表
  - Upload() - 上传HTML文件
  - Edit() - 编辑交通项（名称、颜色）
  - Remove() - 删除交通项及其文件

- `FCloud3.Entities/Transport/TransportItem.cs`
  - 实体模型，存储用户上传的HTML文件引用

#### 修改文件
- `FCloud3.DbContexts/FCloudContext.cs`
  - 添加 `DbSet<TransportItem> TransportItems`

### 2. 前端新增组件

#### 新组件
- `Greeting.vue` - 时间感知的问候组件
  - 根据时间显示不同问候语（早安/午安/晚安）
  - 深夜提醒功能（23:00-04:59），支持按天关闭

- `CurrencyManager.vue` - 架空��币换算器
  - 管理自定义货币及其汇率
  - 货币间转换计算
  - 仅登录用户可编辑

- `TransportManager.vue` - 交通管理器
  - 上传HTML文件展示
  - 编辑交通项名称和颜色
  - 浏览已上传的交通内容

#### 修改文件
- `HomePage.vue` - 集成新组件
  - 导入并使用 Greeting、CurrencyManager、TransportManager

- `src/utils/com/api.ts` - API 客户端
  - 添加 transport API 方法：
    - getList() - 获取列表
    - upload() - 上传文件
    - edit() - 编辑项目
    - remove() - 删除项目

## 功能说明

### TransportManager
- 用户可上传自己的 HTML 文件（支持 .html/.htm 格式）
- 文件存储在 `wwwroot/transports/{userId}/` 下
- 支持为上传文件添加自定义名称和颜色标签
- 仅所有者和管理员可编辑或删除

### CurrencyManager  
- 用户可创建和管理多种架空货币
- 定义汇率比例（相对于用户定义的基准）
- 支持货币间的转换计算
- 仅登录用户可操作

### Greeting
- 自动根据时间显示问候
- 深夜提醒（可按日期关闭）
- 提高用户体验

## 测试建议

1. **后端测试**
   - 测试 Transport CRUD 操作的权限控制
   - 验证文件上传和删除机制
   - 测试 HTML 文件格式验证

2. **前端测试**
   - 测试登录/未登录状态下的界面表现
   - 验证货币转换计算准确性
   - 测试 localStorage 深夜提醒持久化

3. **集成测试**
   - 验证新组件在 HomePage 中正确渲染
   - 测试 API 调用的成功率和错误处理

## 合并步骤

```bash
# 1. 切换到 master 分支
git checkout master

# 2. 合并 feat/greeting-currency
git merge feat/greeting-currency

# 3. 推送到远程
git push origin master

# 4. （可选）删除功能分支
git branch -d feat/greeting-currency
git push origin --delete feat/greeting-currency
```

---

*此文档由 GitHub Copilot 生成*
