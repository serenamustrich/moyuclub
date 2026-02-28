<template>
  <div class="mbti-test">
    <div class="container">
      <h1>MBTI 人格检测</h1>
      
      <!-- 测试说明 -->
      <div class="test-intro" v-if="currentQuestion === 0">
        <h2>测试说明</h2>
        <p>MBTI（Myers-Briggs Type Indicator）是一种基于卡尔·荣格的心理类型理论的人格测试工具。通过回答以下问题，你将了解自己的人格类型。</p>
        <p>请根据你的真实情况选择最符合你的选项，不要思考太久，凭第一感觉回答。</p>
        <button class="btn" @click="startTest">开始测试</button>
      </div>
      
      <!-- 测试问题 -->
      <div class="test-questions" v-else-if="currentQuestion <= questions.length">
        <div class="question-progress">
          <span>{{ currentQuestion - 1 }} / {{ questions.length }}</span>
          <div class="progress-bar">
            <div class="progress" :style="{ width: ((currentQuestion - 1) / questions.length) * 100 + '%' }"></div>
          </div>
        </div>
        
        <div class="question-card">
          <h2>问题 {{ currentQuestion - 1 }}</h2>
          <p>{{ questions[currentQuestion - 2].question }}</p>
          <div class="options">
            <button 
              class="option-btn" 
              :class="{ active: selectedOption === 0 }"
              @click="selectOption(0)"
            >
              {{ questions[currentQuestion - 2].options[0] }}
            </button>
            <button 
              class="option-btn" 
              :class="{ active: selectedOption === 1 }"
              @click="selectOption(1)"
            >
              {{ questions[currentQuestion - 2].options[1] }}
            </button>
          </div>
          <div class="question-actions">
            <button class="btn btn-secondary" v-if="currentQuestion > 2" @click="previousQuestion">上一题</button>
            <button class="btn" @click="nextQuestion" :disabled="selectedOption === null">下一题</button>
          </div>
        </div>
      </div>
      
      <!-- 测试结果 -->
      <div class="test-result" v-else>
        <h2>测试结果</h2>
        <div class="result-card">
          <div class="result-type">
            <h3>{{ resultType }}</h3>
            <p>{{ resultTitle }}</p>
          </div>
          <div class="result-description">
            <p>{{ resultDescription }}</p>
          </div>
          <div class="result-details">
            <h4>人格维度</h4>
            <div class="dimensions">
              <div class="dimension">
                <span>外向 (E) / 内向 (I)</span>
                <div class="dimension-bar">
                  <div class="dimension-fill" :style="{ width: dimensions.EI * 100 + '%' }"></div>
                </div>
                <span>{{ dimensions.EI > 0.5 ? 'E' : 'I' }}</span>
              </div>
              <div class="dimension">
                <span>感觉 (S) / 直觉 (N)</span>
                <div class="dimension-bar">
                  <div class="dimension-fill" :style="{ width: dimensions.SN * 100 + '%' }"></div>
                </div>
                <span>{{ dimensions.SN > 0.5 ? 'S' : 'N' }}</span>
              </div>
              <div class="dimension">
                <span>思考 (T) / 情感 (F)</span>
                <div class="dimension-bar">
                  <div class="dimension-fill" :style="{ width: dimensions.TF * 100 + '%' }"></div>
                </div>
                <span>{{ dimensions.TF > 0.5 ? 'T' : 'F' }}</span>
              </div>
              <div class="dimension">
                <span>判断 (J) / 知觉 (P)</span>
                <div class="dimension-bar">
                  <div class="dimension-fill" :style="{ width: dimensions.JP * 100 + '%' }"></div>
                </div>
                <span>{{ dimensions.JP > 0.5 ? 'J' : 'P' }}</span>
              </div>
            </div>
          </div>
          <button class="btn" @click="restartTest">重新测试</button>
        </div>
      </div>
      
      <div class="footer">
        <button class="btn btn-secondary" @click="$router.push('/')">返回首页</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'MBTITest',
  data() {
    return {
      currentQuestion: 0,
      selectedOption: null,
      answers: [],
      dimensions: {
        EI: 0,
        SN: 0,
        TF: 0,
        JP: 0
      },
      resultType: '',
      resultTitle: '',
      resultDescription: '',
      questions: [
        {
          question: '你更喜欢？',
          options: ['与很多人交往，参加社交活动', '独处或与少数亲密朋友相处'],
          dimension: 'EI',
          weight: [1, 0]
        },
        {
          question: '你更倾向于？',
          options: ['注重事实和细节', '关注整体和可能性'],
          dimension: 'SN',
          weight: [1, 0]
        },
        {
          question: '做决定时，你更依赖？',
          options: ['逻辑和分析', '情感和价值观'],
          dimension: 'TF',
          weight: [1, 0]
        },
        {
          question: '你更喜欢？',
          options: ['有计划、有组织的生活', '灵活、即兴的生活'],
          dimension: 'JP',
          weight: [1, 0]
        },
        {
          question: '你通常？',
          options: ['先行动，再思考', '先思考，再行动'],
          dimension: 'EI',
          weight: [1, 0]
        },
        {
          question: '你更关注？',
          options: ['现实的、具体的事物', '抽象的、理论的概念'],
          dimension: 'SN',
          weight: [1, 0]
        },
        {
          question: '在团队中，你更看重？',
          options: ['公正和客观', '和谐和包容'],
          dimension: 'TF',
          weight: [1, 0]
        },
        {
          question: '你更倾向于？',
          options: ['提前计划，避免意外', '随遇而安，适应变化'],
          dimension: 'JP',
          weight: [1, 0]
        },
        {
          question: '你觉得自己是一个？',
          options: ['健谈的人', '安静的人'],
          dimension: 'EI',
          weight: [1, 0]
        },
        {
          question: '你更相信？',
          options: ['经验和实际证据', '直觉和灵感'],
          dimension: 'SN',
          weight: [1, 0]
        },
        {
          question: '你更重视？',
          options: ['原则和规则', '个人感受和需求'],
          dimension: 'TF',
          weight: [1, 0]
        },
        {
          question: '你更喜欢？',
          options: ['完成任务后再放松', '边工作边娱乐'],
          dimension: 'JP',
          weight: [1, 0]
        }
      ],
      resultTypes: {
        'ESTJ': {
          title: '执行官',
          description: 'ESTJ 类型的人是实际、逻辑、分析型的决策者，他们重视传统和秩序，善于组织和管理，是天生的领导者。'
        },
        'ESFJ': {
          title: '领事',
          description: 'ESFJ 类型的人是热情、友好、有责任感的人，他们重视和谐和人际关系，善于照顾他人，是团队中的支持者。'
        },
        'ENTJ': {
          title: '指挥官',
          description: 'ENTJ 类型的人是自信、果断、有远见的领导者，他们善于制定计划和战略，是天生的组织者和决策者。'
        },
        'ENFJ': {
          title: '主人公',
          description: 'ENFJ 类型的人是热情、理想主义、有感染力的领导者，他们善于理解他人，是天生的激励者和引导者。'
        },
        'ISTJ': {
          title: '检查员',
          description: 'ISTJ 类型的人是实际、可靠、注重细节的人，他们重视传统和秩序，善于执行计划和任务。'
        },
        'ISFJ': {
          title: '守护者',
          description: 'ISFJ 类型的人是温和、体贴、有责任感的人，他们重视和谐和稳定，善于照顾他人的需求。'
        },
        'INTJ': {
          title: '建筑师',
          description: 'INTJ 类型的人是独立、创新、有远见的思考者，他们善于分析和解决复杂问题，是天生的战略家。'
        },
        'INFJ': {
          title: '提倡者',
          description: 'INFJ 类型的人是理想主义、有洞察力、有同情心的人，他们善于理解他人，是天生的顾问和引导者。'
        },
        'ESTP': {
          title: '企业家',
          description: 'ESTP 类型的人是灵活、冒险、行动导向的人，他们善于适应变化，是天生的问题解决者和冒险家。'
        },
        'ESFP': {
          title: '表演者',
          description: 'ESFP 类型的人是热情、活泼、善于社交的人，他们重视当下的体验，是天生的娱乐者和社交达人。'
        },
        'ENTP': {
          title: '辩论家',
          description: 'ENTP 类型的人是好奇、创新、善于分析的人，他们喜欢挑战传统，是天生的思想家和发明家。'
        },
        'ENFP': {
          title: '竞选者',
          description: 'ENFP 类型的人是热情、理想主义、富有创造力的人，他们善于发现可能性，是天生的激励者和创新者。'
        },
        'ISTP': {
          title: '鉴赏家',
          description: 'ISTP 类型的人是冷静、务实、善于动手的人，他们善于解决实际问题，是天生的技术专家和冒险家。'
        },
        'ISFP': {
          title: '探险家',
          description: 'ISFP 类型的人是温和、敏感、富有创造力的人，他们重视个人价值观，是天生的艺术家和自由精神。'
        },
        'INTP': {
          title: '逻辑学家',
          description: 'INTP 类型的人是理性、好奇、善于分析的人，他们喜欢探索理论和概念，是天生的思想家和科学家。'
        },
        'INFP': {
          title: '调停者',
          description: 'INFP 类型的人是理想主义、敏感、富有创造力的人，他们重视个人价值观和意义，是天生的诗人和哲学家。'
        }
      }
    }
  },
  methods: {
    startTest() {
      this.currentQuestion = 2
      this.selectedOption = null
      this.answers = []
      this.dimensions = {
        EI: 0,
        SN: 0,
        TF: 0,
        JP: 0
      }
    },
    selectOption(option) {
      this.selectedOption = option
    },
    previousQuestion() {
      this.currentQuestion--
      this.selectedOption = this.answers[this.currentQuestion - 2]
    },
    nextQuestion() {
      if (this.selectedOption !== null) {
        this.answers.push(this.selectedOption)
        
        // 计算维度得分
        const question = this.questions[this.currentQuestion - 2]
        if (this.selectedOption === 0) {
          this.dimensions[question.dimension] += question.weight[0]
        } else {
          this.dimensions[question.dimension] += question.weight[1]
        }
        
        this.currentQuestion++
        this.selectedOption = null
        
        // 计算测试结果
        if (this.currentQuestion > this.questions.length) {
          this.calculateResult()
        }
      }
    },
    calculateResult() {
      // 计算各维度的比例
      const totalEI = this.answers.filter((answer, index) => 
        this.questions[index].dimension === 'EI'
      ).length
      const totalSN = this.answers.filter((answer, index) => 
        this.questions[index].dimension === 'SN'
      ).length
      const totalTF = this.answers.filter((answer, index) => 
        this.questions[index].dimension === 'TF'
      ).length
      const totalJP = this.answers.filter((answer, index) => 
        this.questions[index].dimension === 'JP'
      ).length
      
      this.dimensions.EI = this.dimensions.EI / totalEI
      this.dimensions.SN = this.dimensions.SN / totalSN
      this.dimensions.TF = this.dimensions.TF / totalTF
      this.dimensions.JP = this.dimensions.JP / totalJP
      
      // 确定人格类型
      let type = ''
      type += this.dimensions.EI > 0.5 ? 'E' : 'I'
      type += this.dimensions.SN > 0.5 ? 'S' : 'N'
      type += this.dimensions.TF > 0.5 ? 'T' : 'F'
      type += this.dimensions.JP > 0.5 ? 'J' : 'P'
      
      this.resultType = type
      this.resultTitle = this.resultTypes[type].title
      this.resultDescription = this.resultTypes[type].description
    },
    restartTest() {
      this.currentQuestion = 0
      this.selectedOption = null
      this.answers = []
      this.dimensions = {
        EI: 0,
        SN: 0,
        TF: 0,
        JP: 0
      }
      this.resultType = ''
      this.resultTitle = ''
      this.resultDescription = ''
    }
  }
}
</script>

<style scoped>
.mbti-test {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.container {
  width: 100%;
  max-width: 800px;
  background: rgba(0, 0, 0, 0.7);
  border-radius: 15px;
  border: 2px solid #9c27b0;
  box-shadow: 0 0 30px rgba(156, 39, 176, 0.3);
  padding: 30px;
}

h1 {
  color: #9c27b0;
  text-shadow: 0 0 20px rgba(156, 39, 176, 0.5);
  font-size: 32px;
  margin-bottom: 30px;
  text-align: center;
}

h2 {
  color: #9c27b0;
  font-size: 24px;
  margin-bottom: 20px;
  text-align: center;
}

.test-intro {
  text-align: center;
  margin-bottom: 40px;
}

.test-intro p {
  margin-bottom: 20px;
  color: #ccc;
  line-height: 1.6;
}

.question-progress {
  margin-bottom: 30px;
}

.question-progress span {
  display: block;
  text-align: center;
  margin-bottom: 10px;
  color: #ccc;
}

.progress-bar {
  width: 100%;
  height: 10px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 5px;
  overflow: hidden;
}

.progress {
  height: 100%;
  background: linear-gradient(90deg, #9c27b0, #e040fb);
  border-radius: 5px;
  transition: width 0.3s ease;
}

.question-card {
  background: rgba(0, 0, 0, 0.5);
  border-radius: 10px;
  padding: 30px;
  margin-bottom: 30px;
}

.question-card h2 {
  text-align: left;
  margin-bottom: 20px;
}

.question-card p {
  font-size: 18px;
  margin-bottom: 30px;
  color: #fff;
  line-height: 1.6;
}

.options {
  display: flex;
  flex-direction: column;
  gap: 15px;
  margin-bottom: 30px;
}

.option-btn {
  padding: 15px 20px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid #333;
  border-radius: 8px;
  color: #fff;
  font-size: 16px;
  text-align: left;
  cursor: pointer;
  transition: all 0.3s ease;
}

.option-btn:hover {
  background: rgba(156, 39, 176, 0.2);
  border-color: #9c27b0;
}

.option-btn.active {
  background: rgba(156, 39, 176, 0.3);
  border-color: #9c27b0;
  box-shadow: 0 0 10px rgba(156, 39, 176, 0.5);
}

.question-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.test-result {
  text-align: center;
}

.result-card {
  background: rgba(0, 0, 0, 0.5);
  border-radius: 10px;
  padding: 40px;
  margin-bottom: 30px;
}

.result-type h3 {
  font-size: 36px;
  color: #9c27b0;
  text-shadow: 0 0 20px rgba(156, 39, 176, 0.5);
  margin-bottom: 10px;
}

.result-type p {
  font-size: 20px;
  color: #ccc;
  margin-bottom: 30px;
}

.result-description {
  margin-bottom: 40px;
}

.result-description p {
  font-size: 16px;
  color: #ccc;
  line-height: 1.6;
  text-align: left;
}

.result-details h4 {
  color: #9c27b0;
  font-size: 18px;
  margin-bottom: 20px;
  text-align: left;
}

.dimensions {
  display: flex;
  flex-direction: column;
  gap: 15px;
  margin-bottom: 30px;
}

.dimension {
  display: flex;
  align-items: center;
  gap: 15px;
}

.dimension span {
  color: #ccc;
  font-size: 14px;
  min-width: 120px;
  text-align: right;
}

.dimension-bar {
  flex: 1;
  height: 8px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
  overflow: hidden;
}

.dimension-fill {
  height: 100%;
  background: linear-gradient(90deg, #9c27b0, #e040fb);
  border-radius: 4px;
  transition: width 0.5s ease;
}

.dimension span:last-child {
  min-width: 20px;
  text-align: left;
  font-weight: bold;
  color: #9c27b0;
}

.footer {
  margin-top: 40px;
  text-align: center;
}

@media (max-width: 768px) {
  .container {
    padding: 20px;
  }
  
  h1 {
    font-size: 24px;
  }
  
  h2 {
    font-size: 20px;
  }
  
  .question-card {
    padding: 20px;
  }
  
  .question-card p {
    font-size: 16px;
  }
  
  .option-btn {
    padding: 12px 16px;
    font-size: 14px;
  }
  
  .result-card {
    padding: 20px;
  }
  
  .result-type h3 {
    font-size: 28px;
  }
  
  .result-type p {
    font-size: 18px;
  }
  
  .dimension span {
    font-size: 12px;
    min-width: 100px;
  }
}
</style>