<template>
  <div class="app-container">
    <div class="filter-container" />
    <el-table
      v-loading="loading"
      :data="list"
      @expand-change="expandHandler"
    >
      <el-table-column type="expand">
        <template slot-scope="scope">
          <el-table
            :data="scope.row.services"
          >
            <el-table-column
              label="ID"
              prop="id"
            />
            <el-table-column
              label="Address"
              prop="address"
            />
            <el-table-column
              label="Port"
              prop="port"
            />
            <el-table-column
              label="Tags"
              prop="tags"
            >
              <template slot-scope="scope">
                <el-tag
                  v-for="t in scope.row.tags"
                  :key="t"
                  style="margin-right:8px;"
                >{{ t }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column
              :label="$t('table.actions')"
              align="center"
              width="220"
              class-name="small-padding fixed-width"
            >
              <template slot-scope="scope">
                <el-button
                  :title="$t('table.delete')"
                  type="danger"
                  size="mini"
                  icon="el-icon-delete"
                  circle
                  @click="deregistHandler(scope.row)"
                />
              </template>
            </el-table-column>
          </el-table>
        </template>
      </el-table-column>
      <el-table-column type="index" />
      <el-table-column
        label="服务名称"
        prop="name"
      />
      <el-table-column
        label="服务标签"
        prop="tags"
      >
        <template slot-scope="scope">
          <el-tag
            v-for="t in scope.row.tags"
            :key="t"
            style="margin-right:8px;"
          >{{ t }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column
        label="服务数量"
        prop="count"
      />
    </el-table>
  </div>
</template>
<script>
import * as api from '@/api/services'
export default {
  name: 'Register',
  data() {
    return {
      loading: false,
      list: [],
      total: 0
    }
  },
  mounted() {
    this.getList()
  },
  methods: {
    getList() {
      this.loading = true
      api.list().then(json => {
        this.list = json.data
        this.total = json.total
        this.loading = false
      }).catch(e => {
        this.loading = false
      })
    },
    expandHandler(row) {
      console.log(row)
    },
    deregistHandler(row) {
      this.$confirm('确认注销该服务？').then(() => {
        console.log('deregist')
        api.deregist(row.id).then(() => {
          this.$message.success('注销成功')
          this.getList()
        })
      })
    }
  }
}
</script>

