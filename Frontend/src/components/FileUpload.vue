<template>
    <div>
        <label for="fileInput">Select HTML file:</label>
        <input type="file" id="fileInput" accept=".htm, .html" ref="fileInputRef">
        <button @click="onSelectedFileUpload">Upload</button>
    </div>
</template>
  
<script setup>
import { ref } from 'vue';
import axios from 'axios';

const fileInputRef = ref(null);
const emit = defineEmits(['taskAdded']);

const onSelectedFileUpload = async () => {
    const selectedFile = fileInputRef.value.files[0];
    if (selectedFile) {
        const fileReader = new FileReader();
        fileReader.onload = async (e) => {
            const fileContent = btoa(e.target.result);
            try {
                const result = await axios.post("/Converter/ConvertFile", {
                    FileName: selectedFile.name,
                    HtmlFileData: fileContent
                });
                emit('taskAdded', result.data, selectedFile.name);
                fileInputRef.value.value = '';
            } catch (error) {
                console.error(error);
            }
        };
        fileReader.readAsText(selectedFile);
    }
}
</script>