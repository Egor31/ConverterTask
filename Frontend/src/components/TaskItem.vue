<template>
    <li>
        <div>{{ taskIndex }}</div>
        <div>{{ task.filename }}</div>
        <button v-if="task.status == 'finished'" @click="downloadPdfFile">Download</button>
        <div v-else>Pending task</div>
        <button v-if="task.action == 'delete'" @click="deleteTask">Delete</button>
    </li>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
    taskIndex: Number,
    task: Object
});

const emit = defineEmits([
    'deleteTask', 
    'downloadPdfFile'
]);

const deleteTask = () => {
    emit('deleteTask', props.taskIndex);
};

const downloadPdfFile = () => {
    emit('downloadPdfFile', props.taskIndex);
};
</script>
  
<style scoped>
li {
    display: grid;
    grid-template-columns: minmax(0, 1fr) minmax(0, 4fr) minmax(0, 2fr) minmax(0, 2fr);
    overflow-wrap: break-word;
}

div {
    border: 1px solid black;
    padding: 10px 5px;
    margin: 5px;
}

button {
    margin: 5px;
}
</style>