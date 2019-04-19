import request from '@/utils/request'

/**
 * 服务列表
 */
export const list = () => {
  return request.get('/api/services')
}

/**
 * 注销服务
 * @param {*} id
 */
export const deregist = id => {
  return request.delete(`/api/services/${id}`)
}
