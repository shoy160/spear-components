<template>
  <el-dialog
    :visible.sync="dialogVisible"
    :title="title"
    width="500px"
  >
    <el-form
      ref="triggerForm"
      :model="model"
      label-width="100px"
    >
      <el-form-item
        :rules="{required:true,message:'请选择定时器类型',trigger:'change'}"
        prop="type"
        label="类型"
      >
        <el-select
          v-model="model.type"
          placeholder="选择类型"
          @change="handleTypeChange"
        >
          <el-option
            :key="1"
            :value="1"
            label="Cron"
          />
          <el-option
            :key="2"
            :value="2"
            label="Simple"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="开始时间">
        <el-date-picker
          v-model="model.start"
          type="datetime"
          placeholder="选择日期时间"
        />
      </el-form-item>
      <el-form-item label="结束时间">
        <el-date-picker
          v-model="model.expired"
          type="datetime"
          placeholder="选择结束时间"
        />
      </el-form-item>
      <el-form-item
        v-if="model.type === 1"
        :rules="{required:true,message:'请输入Cron表达式',trigger:'blur'}"
        label="Cron表达式"
        prop="cron"
      >
        <el-input
          v-model="model.cron"
          placeholder="请输入Cron表达式"
          style="width:220px;"
        />
        <a
          href="http://www.bejson.com/othertools/cron/"
          target="_blank"
          class="link-type"
        >Cron表达式生成</a>
      </el-form-item>
      <template v-if="model.type === 2">
        <el-form-item
          :rules="{required:true,message:'请输入执行间隔',trigger:'change'}"
          prop="interval"
          label="间隔"
        >
          <el-input-number
            v-model="model.interval"
            :min="5"
            placeholder="请输入执行间隔"
            style="width:10rem;"
          />
          秒
        </el-form-item>
        <el-form-item
          :rules="{required:true,message:'请输入执行次数',trigger:'change'}"
          prop="times"
          label="执行次数"
        >
          <el-input-number
            v-model="model.times"
            :min="-1"
            placeholder="请输入执行次数"
            style="width:10rem;"
          />
        </el-form-item>
      </template>
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
import { addTrigger, updateTrigger } from '@/api/jobs'
export default {
  name: 'TriggerCreate',
  props: {
    value: {
      type: Object,
      required: false,
      default: () => {}
    },
    job: {
      type: Object,
      required: true
    },
    show: {
      type: Boolean,
      required: true,
      default: () => false
    }
  },
  data() {
    return {
      title: '创建触发器',
      dialogVisible: this.show,
      create: true,
      model: Object.assign({}, this.value)
    }
  },
  watch: {
    show(val) {
      this.dialogVisible = val
    },
    dialogVisible(val) {
      this.$emit('visibleChange', val)
      if (this.$refs['triggerForm']) {
        this.$refs['triggerForm'].resetFields()
      }
    },
    value(val) {
      this.create = !val || !val.id
      if (this.create) {
        this.model = {}
        this.title = '创建触发器'
      } else {
        this.model = val
        this.title = '编辑触发器'
      }
    }
  },
  methods: {
    handleTypeChange(t) {
      this.$refs['triggerForm'].resetFields()
      this.model.type = t
    },
    handleSave() {
      this.$refs['triggerForm'].validate(valid => {
        if (!valid) {
          return false
        }
        if (this.create) {
          addTrigger(this.job.id, this.model).then(json => {
            this.$message.success(`${this.title}成功`)
            this.dialogVisible = false
            this.$emit('success')
          })
        } else {
          updateTrigger(this.model.id, this.model).then(json => {
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
