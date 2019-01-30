<template>
  <el-dialog
    :visible.sync="dialogVisible"
    :title="title"
    width="50%"
  >
    <el-form label-width="100px">
      <el-form-item label="名称">
        <el-input
          v-model="model.name"
          placeholder="请输入连接名称"
          style="width:20rem;"
        />
      </el-form-item>
      <el-form-item label="编码">
        <el-input
          v-model="model.code"
          placeholder="请输入编码"
          style="width:20rem;"
        />
      </el-form-item>
      <el-form-item label="任务类型">
        <el-select
          v-model="model.provider"
          placeholder="选择类型"
        >
          <el-option
            v-for="item in providers"
            :key="item.key"
            :value="item.key"
            :label="item.value"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="数据库连接">
        <el-input
          v-model="model.connectionString"
          placeholder="请输入数据库连接"
          type="textarea"
        />
      </el-form-item>
    </el-form>
    <span
      slot="footer"
      class="dialog-footer"
    >
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button
        type="primary"
        @click="handleSave"
      >确 定</el-button>
    </span>
  </el-dialog>
</template>
<script>
import { add, edit } from '@/api/database'
export default {
  name: 'DatabaseCreate',
  props: {
    value: {
      type: Object,
      required: false,
      default: () => {}
    },
    show: {
      type: Boolean,
      required: true,
      default: () => false
    }
  },
  data() {
    return {
      title: '创建数据库连接',
      dialogVisible: this.show,
      create: true,
      providers: [
        {
          key: 0,
          value: 'SqlServer'
        },
        {
          key: 1,
          value: 'MySql'
        },
        {
          key: 2,
          value: 'PostgreSql'
        },
        {
          key: 3,
          value: 'SQLite'
        }
      ],
      model: Object.assign({ detail: {}}, this.value)
    }
  },
  watch: {
    show(val) {
      this.dialogVisible = val
    },
    dialogVisible(val) {
      this.$emit('visibleChange', val)
    },
    value(val) {
      this.create = !val || !val.id
      if (this.create) {
        this.model = {
          detail: {}
        }
        this.title = '创建数据库连接'
      } else {
        this.model = val
        this.title = '编辑数据库连接'
      }
    }
  },
  methods: {
    handleSave() {
      if (this.create) {
        add(this.model).then(json => {
          this.$message.success(`${this.title}成功`)
          this.dialogVisible = false
          this.$emit('success')
        })
      } else {
        edit(this.model).then(json => {
          this.$message.success(`${this.title}成功`)
          this.dialogVisible = false
          this.$emit('success')
        })
      }
    }
  }
}
</script>

