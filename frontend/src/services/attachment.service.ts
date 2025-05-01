import api from './api'

export const uploadAttachment = async (file: File): Promise<void> => {
    const formData = new FormData()
    formData.append('file', file)

    await api.post('/api/attachments/upload', formData, {
        headers: {'Content-Type': 'multipart/form-data'}
    })
}

export const downloadAttachment = async (objectName: string): Promise<Blob> => {
    const response = await api.get(`/api/attachments/download/${objectName}`, {
        responseType: 'blob'
    })
    return response.data
}
