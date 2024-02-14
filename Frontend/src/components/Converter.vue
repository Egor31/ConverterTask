<template>
    <FileUpload @taskAdded="addTask" />
    <ul>
        <task-item v-for="[taskIndex, convertationTask] in tasksToConvertFile" :key="taskIndex" :task-index="taskIndex"
            :task="convertationTask" @deleteTask="deleteTask" @downloadPdfFile="downloadPdfFile"></task-item>
    </ul>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { connection } from './SignalrConnection';
import FileUpload from './FileUpload.vue';
import TaskItem from './TaskItem.vue';

const tasksToConvertFile = ref(new Map());

connection.on("taskCompleted", (completedTaskId) => {
    tasksToConvertFile.value.set(
        completedTaskId,
        {
            ...tasksToConvertFile.value.get(completedTaskId),
            status: "finished",
            action: "delete"
        }
    );
    tasksToConvertFile.value = new Map([...tasksToConvertFile.value]);
});

onMounted(async () => {
    try {
        const finishedTasks = await axios.get("/Converter/GetAllFinishedTasks");
        finishedTasks.data.forEach(finishedTask => {
            tasksToConvertFile.value.set(
                finishedTask.convertTaskId,
                {
                    filename: finishedTask.filename,
                    status: "finished",
                    action: "delete"
                }
            );
        });
    } catch (error) {
        console.error(error);
    }
});

const addTask = (taskId, filename) => {
    tasksToConvertFile.value.set(taskId, {
        filename,
        status: "uploading",
        action: "stop"
    });
};

const deleteTask = async (taskId) => {
    await axios.get(`/Converter/DeleteTaskById?deletingTaskId=${taskId}`);
    tasksToConvertFile.value.delete(taskId);
    tasksToConvertFile.value = new Map([...tasksToConvertFile.value]);
};

const downloadPdfFile = async (taskIndex) => {
    let response = await axios({
        url: `/Converter/GetPdfFileByFinishedTaskId?finishedTaskId=${taskIndex}`,
        method: 'GET',
        responseType: 'blob'
    });
    var fileURL = window.URL.createObjectURL(new Blob([response.data]));
    var fileLink = document.createElement('a');
    fileLink.href = fileURL;
    fileLink.setAttribute('download', tasksToConvertFile.value.get(taskIndex).filename + ".pdf");
    document.body.appendChild(fileLink);
    fileLink.click();
};
</script>

<style scoped>
ul {
    padding-left: 0px;
}
</style>