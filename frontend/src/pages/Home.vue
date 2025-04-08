<template>
  <div class="home">
    <h1>Мессенджер</h1>
    <router-link to="/login">Перейти к входу</router-link>
  </div>
</template>

<style scoped>
.home {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 30vh;
  gap: 20px;
}
a {
  text-decoration: none;
  color: #42b983;
  font-weight: bold;
}
</style>
