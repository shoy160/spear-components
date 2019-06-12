<template>
  <el-dialog
    :visible.sync="dialogVisible"
    :title="title"
    width="50%">
    <el-form ref="jobForm" :model="model" :rules="rules" label-width="100px">
      <el-form-item label="任务组名" prop="group">
        <el-input v-model="model.group" placeholder="请输入任务组名" style="width:20rem;" />
      </el-form-item>
      <el-form-item label="任务名称" prop="name">
        <el-input v-model="model.name" placeholder="请输入任务组名" style="width:20rem;" />
      </el-form-item>
      <el-form-item label="任务类型" prop="type">
        <el-select v-model="model.type" placeholder="选择任务类型">
          <el-option :key="0" :value="0" label="Http"/>
        </el-select>
      </el-form-item>
      <el-form-item :rules="{ required: true, message: '请输入请求地址', trigger: 'blur' }" prop="detail.url" label="请求地址">
        <el-input v-model="model.detail.url" placeholder="请输入请求地址" />
      </el-form-item>
      <el-form-item :rules="{ required: true, message: '请选择请求类型', trigger: 'change' }" prop="detail.method" label="请求类型">
        <el-radio v-model="model.detail.method" :label="0">Get</el-radio>
        <el-radio v-model="model.detail.method" :label="1">Post</el-radio>
        <el-radio v-model="model.detail.method" :label="2">Delete</el-radio>
        <el-radio v-model="model.detail.method" :label="3">Put</el-radio>
        <el-radio v-model="model.detail.method" :label="4">Patch</el-radio>
      </el-form-item>
      <el-form-item label="请求参数">
        <el-input v-model="model.detail.data" placeholder="请输入请求参数" type="textarea" />
      </el-form-item>
      <el-form-item label="任务描述">
        <el-input v-model="model.desc" placeholder="请输入任务描述" type="textarea" />
      </el-form-item>
    </el-form>
    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="handleSave">确 定</el-button>
    </span>
  </el-dialog>
</template>
<script>
import { create, save } from '@/api/jobs'
export default {
  name: 'JobCreate',
  props: {
    value: {
      type: Object,
      required: false,
      default: function() {
        return {
          detail: {}
        }
      }
    },
    show: {
      type: Boolean,
      required: true,
      default: () => false
    }
  },
  data() {
    return {
      title: '创建定时任务',
      dialogVisible: this.show,
      create: true,
      model: Object.assign({ detail: {}}, this.value),
      rules: {
        group: [
          { required: true, message: '请输入任务分组', trigger: 'blur' }
        ],
        name: [
          { required: true, message: '请输入任务名称', trigger: 'blur' }
        ],
        type: [
          { required: true, message: '请选择任务类型', trigger: 'change' }
        ]
      }
    }
  },
  watch: {
    show(val) {
      this.dialogVisible = val
    },
    dialogVisible(val) {
      this.$emit('visibleChange', val)
      if (this.$refs['jobForm']) {
        this.$refs['jobForm'].resetFields()
      }
    },
    value(val) {
      this.create = !val || !val.id
      if (this.create) {
        this.model = {
          detail: {}
        }
        this.title = '创建定时任务'
      } else {
        this.model = val
        this.title = '编辑定时任务'
      }
    }
  },
  methods: {
    handleSave() {
      this.$refs['jobForm'].validate(valid => {
        if (!valid) {
          return false
        }
        if (this.create) {
          create(this.model).then(json => {
            this.$message.success(`${this.title}成功`)
            this.dialogVisible = false
            this.$emit('success')
          })
        } else {
          save(this.model).then(json => {
            this.$message.success(`${this.title}成功`)
            this.dialogVisible = false
            this.$emit('success')
          })
        }
      })
    }
  }
}
</script>

