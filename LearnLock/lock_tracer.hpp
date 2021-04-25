#pragma once 

#include <map>
#include <string>
#include <thread>

namespace learn {

class lock_thread_tracer;

class lock_tracer
{
public: 

  static lock_tracer inst;

private: 
  lock_tracer()
  {}

public:
  ~lock_tracer()
  {}

  void add(lock_thread_tracer* tracer);

  std::string to_string(std::thread::id id) const;

  std::string to_string() const;

private:
  std::map<std::thread::id, lock_thread_tracer*> tracers_;
};

} // learn