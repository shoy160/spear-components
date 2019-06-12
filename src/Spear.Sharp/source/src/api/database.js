import request from '@/utils/request'

/**
 * 列表
 * @param {Object} search
 */
export const list = ({ keyword, type, page = 1, size = 10 }) => {
  return request.get('/api/database', {
    params: {
      keyword,
      type,
      page,
      size
    }
  })
}

/**
 * 添加
 * @param {*} param0
 */
export const add = ({ name, code, provider, connectionString }) => {
  return request.post('/api/database', { name, code, provider, connectionString })
}

/**
 * 修改
 * @param {*} id
 * @param {*} name
 * @param {*} provider
 * @param {*} connectionString
 */
export const edit = ({ id, name, code, provider, connectionString }) => {
  return request.put(`/api/database/${id}`, {
    name, code, provider, connectionString
  })
}

/**
 * 删除
 * @param {*} id
 */
export const remove = id => {
  return request.delete(`/api/database/${id}`)
}

export const viewUrl = row => {
  return `${process.env.BASE_API}/tables/${row.code}`
}
