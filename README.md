





# Desktop Learning Assistant

# 											——桌面学习助手

.Net架构程序设计大作业



## 四周课程安排：

第七周（10.19）：检查各个小组选题的需求、边界、技术可行性。
第八周（10.26）：检查各个小组的架构设计与代码框架。
第九周（11.2）：检查各个小组代码中存在的问题。
第十周（11.9）：检查各个小组的功能完成与集成情况。
第十一周（11.16）：使用腾讯会议组织一次公开展示，每个小组讲解并展示自己的作品。



## TODO：

### 团队任务：

- [ ] 安排答辩人
- [ ] 排练答辩



### 总的UI界面：

- [ ] 各个按钮：素材高清化；停靠、点击高亮等
- [ ] 字体统一
- [ ] 透明度调整
- [ ] 主界面拖拽、桌面停靠
- [ ] 从系统中读出软件图标



### 屏幕使用时间统计部分：

今日/昨日/一周统计：

- [ ] 解决频闪问题
- [ ] 对接数据更新事件
- [ ] 完成一周时间图

效率状态统计：

- [ ] 提供一周的效率统计（ms）：界面重新设计

后台部分：

- [ ] 使用win32事件代替轮询
- [ ] 将服务类的接口写为异步方法

- [ ] 提供数据改变的事件，通知界面更新ViewModel



### 番茄钟&任务管理部分：

**番茄钟界面：**

- [ ] 按钮透明、素材高清
- [ ] 更好的进度条
- [ ] 钟下方软件列表用listview，且显示时长
- [ ] 所有任务的图标不应是“…”

**所有任务界面：**

- [ ] 用一个界面显示所有待完成任务和已完成任务
- [ ] 添加任务、删除任务、修改任务、查询任务功能
- [ ] 每个textbox应有检查输入的功能

**后台：**

- [ ] task实体类添加白名单列表



### 文件管理部分：

- [ ] ……



### 配置部分：

**界面：**

- [ ] 添加白名单、删除白名单、更改白名单
- [ ] 番茄钟各个属性配置
- [ ] 更好的软件类型设置方式

**后台：**

- [ ] 要求所有配置类实现SetDefault接口，用以设置默认的配置
- [ ] 提供默认白名单模板
- [ ] 把番茄钟新的配置类放入 Configuration