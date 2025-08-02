import nodemailer from 'nodemailer'
import dotenv from 'dotenv'

dotenv.config()

class EmailService {
  constructor() {
    this.transporter = null
    this.init()
  }

  init() {
    // For development, we'll use a test account
    // In production, you would use real SMTP credentials
    this.transporter = nodemailer.createTransport({
      host: 'smtp.gmail.com',
      port: 587,
      secure: false, // true for 465, false for other ports
      auth: {
        user: process.env.EMAIL_USER || 'your-email@gmail.com',
        pass: process.env.EMAIL_PASS || 'your-app-password'
      }
    })
  }

  async sendVerificationEmail(email, firstName, verificationToken) {
    try {
      // Use environment variable for base URL, fallback to localhost for development
      const baseUrl = process.env.BASE_URL || 'https://alientranslate.ru'
      const verificationUrl = `${baseUrl}/verify-email/${verificationToken}`
      
      const mailOptions = {
        from: process.env.EMAIL_USER || 'noreply@alientranslate.com',
        to: email,
        subject: 'Подтвердите ваш email - AlienTranslate',
        html: `
          <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <div style="background: #161616; color: #ffffff; padding: 20px; text-align: center;">
              <h1 style="color: #38b7a1; margin: 0;">AlienTranslate</h1>
            </div>
            
            <div style="background: #ffffff; padding: 30px; color: #333;">
              <h2>Привет, ${firstName}!</h2>
              
              <p>Спасибо за регистрацию в AlienTranslate. Для завершения регистрации и получения доступа к вашему токену пользователя, пожалуйста, подтвердите ваш email адрес.</p>
              
              <div style="text-align: center; margin: 30px 0;">
                <a href="${verificationUrl}" 
                   style="background: #38b7a1; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;">
                  Подтвердить Email
                </a>
              </div>
              
              <p>Или скопируйте эту ссылку в браузер:</p>
              <p style="word-break: break-all; background: #f5f5f5; padding: 10px; border-radius: 3px;">
                ${verificationUrl}
              </p>
              
              <p><strong>Важно:</strong> Эта ссылка действительна в течение 24 часов.</p>
              
              <hr style="margin: 30px 0; border: none; border-top: 1px solid #eee;">
              
              <p style="font-size: 12px; color: #666;">
                Если вы не регистрировались в AlienTranslate, просто проигнорируйте это письмо.
              </p>
            </div>
          </div>
        `
      }

      const info = await this.transporter.sendMail(mailOptions)
      console.log('Verification email sent:', info.messageId)
      return true
    } catch (error) {
      console.error('Error sending verification email:', error)
      return false
    }
  }
}

export default new EmailService() 