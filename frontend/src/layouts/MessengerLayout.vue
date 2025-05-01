<template>
  <div class="layout">
    <aside class="sidebar">
      <slot name="sidebar"/>
    </aside>
    <main class="main">
      <slot name="main"/>
    </main>
  </div>
</template>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
  background: #1e1f22;
  color: white;
}

.sidebar {
  width: 300px;
  background: #2b2d31;
  overflow-y: auto;
  border-right: 1px solid #444;
}

.main {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  position: relative;
}
</style>
