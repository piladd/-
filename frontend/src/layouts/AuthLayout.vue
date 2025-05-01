<template>
  <div class="auth-layout">
    <slot/>
  </div>
</template>

<style scoped>
.auth-layout {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: linear-gradient(to right, #1d1f27, #20232a);
}
</style>
