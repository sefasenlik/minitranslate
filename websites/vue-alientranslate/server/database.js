import sqlite3 from 'sqlite3'
import { fileURLToPath } from 'url'
import { dirname, join } from 'path'

const __filename = fileURLToPath(import.meta.url)
const __dirname = dirname(__filename)

class Database {
  constructor() {
    this.db = null
    // Use the data directory for the database file
    this.dbPath = join(__dirname, '../data/users.db')
  }

  // Initialize database and create tables
  async init() {
    return new Promise((resolve, reject) => {
      this.db = new sqlite3.Database(this.dbPath, (err) => {
        if (err) {
          console.error('Error opening database:', err.message)
          reject(err)
          return
        }
        
        console.log('Connected to SQLite database')
        this.createTables().then(resolve).catch(reject)
      })
    })
  }

  // Create users table
  async createTables() {
    return new Promise((resolve, reject) => {
      const createTableSQL = `
        CREATE TABLE IF NOT EXISTS users (
          id TEXT PRIMARY KEY,
          firstName TEXT NOT NULL,
          lastName TEXT NOT NULL,
          email TEXT UNIQUE NOT NULL,
          password TEXT NOT NULL,
          userToken TEXT UNIQUE NOT NULL,
          emailVerified BOOLEAN DEFAULT FALSE,
          emailVerificationToken TEXT,
          createdAt TEXT NOT NULL,
          lastLogin TEXT
        )
      `

      this.db.run(createTableSQL, (err) => {
        if (err) {
          console.error('Error creating table:', err.message)
          reject(err)
          return
        }
        
        console.log('Users table ready')
        resolve()
      })
    })
  }

  // Create a new user
  async createUser(userData) {
    return new Promise((resolve, reject) => {
      const sql = `
        INSERT INTO users (id, firstName, lastName, email, password, userToken, emailVerified, emailVerificationToken, createdAt, lastLogin)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
      `
      
      const params = [
        userData.id,
        userData.firstName,
        userData.lastName,
        userData.email,
        userData.password,
        userData.userToken,
        userData.emailVerified || false,
        userData.emailVerificationToken || null,
        userData.createdAt,
        userData.lastLogin
      ]

      this.db.run(sql, params, function(err) {
        if (err) {
          console.error('Error creating user:', err.message)
          reject(err)
          return
        }
        
        resolve({ id: this.lastID, ...userData })
      })
    })
  }

  // Find user by email
  async findUserByEmail(email) {
    return new Promise((resolve, reject) => {
      const sql = 'SELECT * FROM users WHERE email = ?'
      
      this.db.get(sql, [email], (err, row) => {
        if (err) {
          console.error('Error finding user:', err.message)
          reject(err)
          return
        }
        
        resolve(row || null)
      })
    })
  }

  // Find user by ID
  async findUserById(id) {
    return new Promise((resolve, reject) => {
      const sql = 'SELECT * FROM users WHERE id = ?'
      
      this.db.get(sql, [id], (err, row) => {
        if (err) {
          console.error('Error finding user:', err.message)
          reject(err)
          return
        }
        
        resolve(row || null)
      })
    })
  }

  // Update user profile
  async updateUser(id, updateData) {
    return new Promise((resolve, reject) => {
      const fields = []
      const values = []
      
      if (updateData.firstName !== undefined) {
        fields.push('firstName = ?')
        values.push(updateData.firstName)
      }
      
      if (updateData.lastName !== undefined) {
        fields.push('lastName = ?')
        values.push(updateData.lastName)
      }
      
      if (updateData.email !== undefined) {
        fields.push('email = ?')
        values.push(updateData.email)
      }
      
      if (updateData.password !== undefined) {
        fields.push('password = ?')
        values.push(updateData.password)
      }
      
      if (updateData.lastLogin !== undefined) {
        fields.push('lastLogin = ?')
        values.push(updateData.lastLogin)
      }
      
      if (fields.length === 0) {
        resolve(null)
        return
      }
      
      values.push(id)
      const sql = `UPDATE users SET ${fields.join(', ')} WHERE id = ?`
      
      this.db.run(sql, values, function(err) {
        if (err) {
          console.error('Error updating user:', err.message)
          reject(err)
          return
        }
        
        resolve({ changes: this.changes })
      })
    })
  }

  // Check if email exists (for registration validation)
  async emailExists(email) {
    return new Promise((resolve, reject) => {
      const sql = 'SELECT COUNT(*) as count FROM users WHERE email = ?'
      
      this.db.get(sql, [email], (err, row) => {
        if (err) {
          console.error('Error checking email:', err.message)
          reject(err)
          return
        }
        
        resolve(row.count > 0)
      })
    })
  }

  // Get all users (for debugging/admin purposes)
  async getAllUsers() {
    return new Promise((resolve, reject) => {
      const sql = 'SELECT id, firstName, lastName, email, userToken, emailVerified, createdAt, lastLogin FROM users'
      
      this.db.all(sql, [], (err, rows) => {
        if (err) {
          console.error('Error getting users:', err.message)
          reject(err)
          return
        }
        
        resolve(rows)
      })
    })
  }

  // Verify email with token
  async verifyEmail(verificationToken) {
    return new Promise((resolve, reject) => {
      const sql = `
        UPDATE users 
        SET emailVerified = TRUE, emailVerificationToken = NULL 
        WHERE emailVerificationToken = ?
      `
      
      this.db.run(sql, [verificationToken], function(err) {
        if (err) {
          console.error('Error verifying email:', err.message)
          reject(err)
          return
        }
        
        resolve({ changes: this.changes })
      })
    })
  }

  // Find user by verification token
  async findUserByVerificationToken(token) {
    return new Promise((resolve, reject) => {
      const sql = 'SELECT * FROM users WHERE emailVerificationToken = ?'
      
      this.db.get(sql, [token], (err, row) => {
        if (err) {
          console.error('Error finding user by verification token:', err.message)
          reject(err)
          return
        }
        
        resolve(row || null)
      })
    })
  }

  // Close database connection
  close() {
    if (this.db) {
      this.db.close((err) => {
        if (err) {
          console.error('Error closing database:', err.message)
        } else {
          console.log('Database connection closed')
        }
      })
    }
  }
}

export default new Database() 